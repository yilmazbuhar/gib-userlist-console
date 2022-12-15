using GibUsers.Api.ElasticSearch;
using Nest;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace GibUsers.Api
{
    public class ElasticSyncService : ISyncService, IDisposable
    {
        private readonly IElasticService _elasticsearchService;
        public ElasticSyncService(IElasticService elasticService)
        {
            _elasticsearchService = elasticService;
        }

        public async Task SyncDataAsync(Stream stream)
        {
            var list = Enumerable.Chunk(Data(stream), 5000);
            
            foreach (var item in list)
            {
                await BulkIndex(item.ToList());
            }
        }

        IEnumerable<UserJsonModel> Data(Stream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name.ToString() == "User")
                    {
                        foreach (var item in AddUserFromXmlNode(reader.ReadOuterXml()))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        IEnumerable<UserJsonModel> AddUserFromXmlNode(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                yield return null;

            var serializer = new XmlSerializer(typeof(UserXml));

            using (TextReader reader = new StringReader(xml))
            {
                var userXml = (UserXml)serializer.Deserialize(reader);

                if (userXml == null)
                    yield return null;

                UserJsonModel baseuser = new UserJsonModel(userXml);

                foreach (var doc in userXml.Documents.Document)
                {
                    var alias = doc.Alias.FirstOrDefault(x => x.DeletionTime == null) ??
                        doc.Alias.OrderByDescending(x => x.DeletionTime).FirstOrDefault();

                    var user = (UserJsonModel)baseuser.Clone();

                    if (alias?.DeletionTime != null)
                        user.DeactivateDate = alias.DeletionTime;

                    user.AliasCreationTime = alias.CreationTime;
                    user.AppType = doc.Type;
                    user.Alias = alias.Name;

                    // set id programmatically to update existing data
                    // we assume that the Identifier is unique
                    user.Id = $"{userXml.Identifier}{doc.Type.ToLower(CultureInfo.InvariantCulture)}";

                    yield return user;
                }
            }
        }

        async Task<BulkResponse> BulkIndex(List<UserJsonModel>? users)
        {
            return await _elasticsearchService.BulkIndex(users);
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}

using Microsoft.Extensions.Options;
using Nest;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace GibUsers.Api
{
    public interface ISyncService
    {
        Task SyncDataAsync();
    }


    public abstract class BaseElasticSyncService : ISyncService, IDisposable
    {
        private readonly IGibDataService _gibDataService;
        //private Stream _dataStream;
        
        private readonly ElasticSearchConfig _elasticSearchConfig;
        private readonly ElasticClient _elasticsearchClient;
        public BaseElasticSyncService(IGibDataService gibDataService,
            ElasticClient elasticClient,
            IOptions<ElasticSearchConfig> elasticSearchConfig)
        {
            _gibDataService = gibDataService;
            _elasticsearchClient = elasticClient;
            _elasticSearchConfig = elasticSearchConfig.Value;
        }

        public abstract Task<Stream> GetData();

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

        async Task BulkIndex(List<UserJsonModel>? users)
        {
            await _elasticsearchClient.IndexManyAsync(users);
        }

        public async Task SyncDataAsync()
        {
            var dataStream = await GetData();
            List<UserJsonModel>? users = new List<UserJsonModel>();

            using (XmlReader reader = XmlReader.Create(dataStream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name.ToString() == "User")
                    {
                        users.AddRange(AddUserFromXmlNode(await reader.ReadOuterXmlAsync()));

                        if (users.Count >= _elasticSearchConfig.BulkInsertCount)
                        {
                            await BulkIndex(users);
                            users = new List<UserJsonModel>();
                        }
                    }
                }
                await BulkIndex(users);
                
                users = null;
            }
        }

        public void Dispose()
        {
            this.Dispose();
        }
    }
}

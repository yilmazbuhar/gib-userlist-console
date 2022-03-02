// See https://aka.ms/new-console-template for more information
using GibUserSync;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;


var (serviceProvider,config) = ConfigureServices.Configure();
var elasticClient = serviceProvider.GetService<ElasticClient>();
ElasticSearchConfig elasticSearchConfig = config.GetRequiredSection("ElasticSearchConfig").Get<ElasticSearchConfig>();

using (XmlReader reader = XmlReader.Create(@"gibusers.xml"))
{
    List<UserJsonModel> users = new List<UserJsonModel>();
    while (reader.Read())
    {
        if (reader.IsStartElement() && reader.Name.ToString() == "User")
        {
            users.AddRange(GetUser(reader.ReadOuterXml()));
            //if (user == null)
            //    continue;
            if (users.Count >= elasticSearchConfig.BulkInsertCount)
            {
                await elasticClient.IndexManyAsync(users);
                users = new List<UserJsonModel>();
            }
            //elasticClient.Indices.Refresh();
        }
    }

    users = null;
}
Console.ReadKey();

List<UserJsonModel>? GetUser(string xml)
{
    if (string.IsNullOrEmpty(xml))
        return null;

    var serializer = new XmlSerializer(typeof(UserXml));

    using (TextReader reader = new StringReader(xml))
    {
        var userXml = (UserXml)serializer.Deserialize(reader);

        if (userXml == null)
            return null;

        List<UserJsonModel> users = new List<UserJsonModel>();
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

            users.Add(user);
        }

        return users;
    }
}
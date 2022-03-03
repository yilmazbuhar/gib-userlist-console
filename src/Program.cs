// See https://aka.ms/new-console-template for more information
using GibUserSync;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;


var (serviceProvider, config) = ConfigureServices.Configure();
var elasticClient = serviceProvider.GetService<ElasticClient>();
ElasticSearchConfig elasticSearchConfig = config.GetRequiredSection("ElasticSearchConfig").Get<ElasticSearchConfig>();

//Channel<List<UserJsonModel>> channel = Channel.CreateUnbounded<List<UserJsonModel>>();

List<UserJsonModel> users = new List<UserJsonModel>();
Console.WriteLine(await StopwatchAction(async () =>
{
    using (XmlReader reader = XmlReader.Create(@"gibusers.xml"))
    {
        while (reader.Read())
        {
            if (reader.IsStartElement() && reader.Name.ToString() == "User")
            {
                AddUserFromXmlNode(reader.ReadOuterXml());

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
}));

Console.ReadKey();

List<UserJsonModel>? AddUserFromXmlNode(string xml)
{
    if (string.IsNullOrEmpty(xml))
        return null;

    var serializer = new XmlSerializer(typeof(UserXml));

    using (TextReader reader = new StringReader(xml))
    {
        var userXml = (UserXml)serializer.Deserialize(reader);

        if (userXml == null)
            return null;

        UserJsonModel baseuser = new UserJsonModel(userXml);
        userXml.Documents.Document.ForEach(x =>
        {

        });

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

async Task<double> StopwatchAction(Func<Task> callback)
{
    Stopwatch sw = Stopwatch.StartNew();
    await callback();
    sw.Stop();

    return sw.Elapsed.TotalSeconds;
}
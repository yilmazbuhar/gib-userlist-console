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
var _elasticClient = serviceProvider.GetService<ElasticClient>();
var _gibDownloadService = serviceProvider.GetService<IGibDownloadService>();
ElasticSearchConfig elasticSearchConfig = config.GetRequiredSection("ElasticSearchConfig").Get<ElasticSearchConfig>();

//Channel<List<UserJsonModel>> channel = Channel.CreateUnbounded<List<UserJsonModel>>();


//Console.WriteLine(await StopwatchAction(async () =>
//{
    
//}));

Console.ReadKey();

async Task BulkIndex()
{
    if (users.Count < elasticSearchConfig.BulkInsertCount)
        return;

    await _elasticClient.IndexManyAsync(users);
    users = new List<UserJsonModel>();
}



async Task<double> StopwatchAction(Func<Task> callback)
{
    Stopwatch sw = Stopwatch.StartNew();
    await callback();
    sw.Stop();

    return sw.Elapsed.TotalSeconds;
}
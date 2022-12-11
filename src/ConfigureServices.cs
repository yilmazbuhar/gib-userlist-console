using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace GibUserSync
{
    public static class ConfigureServices
    {
        public static (IServiceProvider, IConfigurationRoot) Configure()
        {
            var services = new ServiceCollection();

            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            services.AddSingleton(config);
            services.AddHttpClient<IGibDownloadService, GibDownloadService>(client =>
            {
                client.BaseAddress = new Uri(config["Application:EfaturaServiceUri"]);
            });

            ElasticSearchConfig elasticConfig = config.GetRequiredSection("ElasticSearchConfig").Get<ElasticSearchConfig>();

            var settings = new ConnectionSettings(new Uri(elasticConfig.Host)).DefaultIndex(elasticConfig.Index);
            var client = new ElasticClient(settings);
            
            services.AddSingleton(client);

            return (services.BuildServiceProvider(), config);
        }
    }
}

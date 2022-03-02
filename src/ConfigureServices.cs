using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace GibUserSync
{
    public static class ConfigureServices
    {
        public static IServiceProvider Configure()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("gibusers");
            var client = new ElasticClient(settings);
            
            var services = new ServiceCollection();
            services.AddSingleton(client);

            return services.BuildServiceProvider();
        }
    }
}

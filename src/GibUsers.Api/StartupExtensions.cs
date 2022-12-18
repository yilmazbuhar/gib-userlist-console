using GibUsers.Api.ElasticSearch;
using Hangfire;
using Microsoft.Extensions.Options;
using Nest;

namespace GibUsers.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            //Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            //Hangfire
            services.AddHangfire(x => x.UseSqlServerStorage(configuration.GetConnectionString("Hangfire")));
            services.AddHangfireServer();

            // Httpclit for gib data
            services.AddHttpClient<IGibDataService, GibDataService>(client =>
            {
                client.BaseAddress = new Uri(configuration["Application:EfaturaServiceUri"]);
            });

            // Elastic config
            services.AddSingleton<ISyncService, ElasticSyncService>();
            services.AddSingleton<IHangfireJobs, HangfireJobs>();
            services.AddSingleton<IElasticService, ElasticService>();

            return services;
        }

        public static IServiceCollection AddElasticClient(this IServiceCollection services, IConfiguration configuration)
        {
            var optionsSection = configuration.GetSection("ElasticSearchConfig");
            services.Configure<ElasticSearchConfig>(optionsSection);

            var serviceProvider = services.BuildServiceProvider();

            var elasticConfig = serviceProvider.GetRequiredService<IOptions<ElasticSearchConfig>>().Value;
            //ElasticSearchConfig elasticConfig1 = configuration.GetRequiredSection("ElasticSearchConfig").Get<ElasticSearchConfig>();
            var settings = new ConnectionSettings(new Uri(elasticConfig.Host)).DefaultIndex(elasticConfig.Index);
            if (!string.IsNullOrEmpty(elasticConfig.Username) && !string.IsNullOrEmpty(elasticConfig.Password))
            {
                settings.BasicAuthentication(elasticConfig.Username, elasticConfig.Password);
            }

            //settings.EnableApiVersioningHeader();
            var client = new ElasticClient(settings);

            services.AddSingleton(client);

            return services;
        }
    }
}

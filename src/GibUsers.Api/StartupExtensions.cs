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

            // Application services
            services.AddSingleton<ISyncService, ElasticSyncService>();
            services.AddSingleton<IHangfireJobs, HangfireJobs>();
            services.AddSingleton<IElasticService, ElasticService>();

            return services;
        }

        public static void AddHangFireJobs(this IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var hangfirejob = serviceProvider.GetService<IHangfireJobs>();
            var minuteInterval = Convert.ToInt32(configuration["Application:HangfireMinuteInterval"]);

            RecurringJob.AddOrUpdate<IHangfireJobs>(service => hangfirejob.GibGbUsersSync(), Cron.MinuteInterval(minuteInterval));
            RecurringJob.AddOrUpdate<IHangfireJobs>(service => hangfirejob.GibPkUsersSync(), Cron.MinuteInterval(minuteInterval));
        }

        public static IServiceCollection AddElasticClient(this IServiceCollection services, IConfiguration configuration)
        {
            var optionsSection = configuration.GetSection("ElasticSearchConfig");
            services.Configure<ElasticSearchConfig>(optionsSection);

            var serviceProvider = services.BuildServiceProvider();

            var elasticConfig = serviceProvider.GetRequiredService<IOptions<ElasticSearchConfig>>().Value;
            
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

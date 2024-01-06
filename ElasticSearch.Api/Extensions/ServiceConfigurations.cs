using ElasticSearchApi.Data.Configurations;
using ElasticSearchApi.Data.Implementations;
using ElasticSearchApi.Data.Interface;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using RabbitMQ.Client;

namespace ElasticSearch.Api.Extensions
{
    public static class ServiceConfigurations
    {
        public static void ConfigureElastic(this IServiceCollection services, IConfiguration configuration) =>
            services.Configure<ElasticSettings>(configuration.GetSection("Elastic"));

        public static void ConfigureElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["Elastic:Cloud"] ?? string.Empty;

            var settings = new ConnectionSettings(new Uri(url))
                .PrettyJson();

            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
        }

        public static void ConfigureRabbitMq(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration["RabbitMq:Connection"] ?? string.Empty;
            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(connection),
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            var client = connectionFactory.CreateConnection("LoggerClient");
            var model = client.CreateModel();
            services.AddSingleton<IConnection>(client);
            services.AddSingleton<IModel>(model);
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();
    }
}

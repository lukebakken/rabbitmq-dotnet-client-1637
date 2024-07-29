using Azure.Storage.Files.Shares;
using RabbitMQ.Client;
using RabbitMQHelper;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace TestWebAPI.Helper
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRabbitMqClientFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    DispatchConsumersAsync = true,
                    RequestedHeartbeat = TimeSpan.FromSeconds(30),
                };

                return factory;
            });
        }
    }
}

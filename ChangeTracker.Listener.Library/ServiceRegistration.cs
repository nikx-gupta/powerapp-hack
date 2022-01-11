using System.Net.Http.Headers;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Listener.Library
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(provider => config.Get<QueueClientConfig>());
            services.AddSingleton<QueueClient>(provider =>
            {
                var settings = provider.GetService<QueueClientConfig>();
                return new QueueClient(settings.QueueConnectionString, settings.QueueName);
            });

            services.AddSingleton(typeof(CsvOutputWriter<>));
        }
    }
}
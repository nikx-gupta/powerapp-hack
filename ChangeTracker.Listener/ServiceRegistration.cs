using ChangeTracking.Clients;
using ChangeTracking.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Listener
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            services.RegisterQueueWriter(config.Get<QueueClientConfig>());
            services.RegisterCsvFormatter(config.Get<CsvWriterSettings>());
        }
    }
}
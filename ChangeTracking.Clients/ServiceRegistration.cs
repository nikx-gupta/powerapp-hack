using System.Data.SqlClient;
using Azure.Storage.Queues;
using ChangeTracking.Clients.Cloud;
using ChangeTracking.Clients.Configuration;
using ChangeTracking.Clients.Formatters;
using ChangeTracking.Clients.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracking.Clients
{
    public static class ServiceRegistration
    {
        public static void RegisterSqlWriters(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.Get<SqlWriterSettings>());
            services.AddSingleton(typeof(SqlWriter<>));
        }

        public static void RegisterQueueClient(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.Get<QueueClientConfig>());
            services.AddSingleton<QueueClient>(provider =>
            {
                var settings = provider.GetService<QueueClientConfig>();
                return new QueueClient(settings.QueueConnectionString, settings.QueueName);
            });

            services.AddSingleton(typeof(QueueWriter));
        }

        public static void RegisterCsvFormatter(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.Get<CsvWriterSettings>());
            services.AddSingleton(typeof(CsvFormatter<>));
        }

        public static void RegisterSqlChangeTracking(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.Get<SqlChangeTrackingSettings>());
            services.AddSingleton<SqlConnection>(provider =>
                new SqlConnection(provider.GetRequiredService<SqlChangeTrackingSettings>().SourceConnectionString));
            services.AddSingleton(typeof(SqlChangeTrackingClient<>));
        }
    }
}

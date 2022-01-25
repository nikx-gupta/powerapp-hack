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
        public static void RegisterWriters(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.Get<CsvWriterSettings>());
            services.AddSingleton(typeof(CsvFormatter<>));

            services.AddSingleton(config.Get<SqlWriterSettings>());
            services.AddSingleton(typeof(SqlWriter<>));

            services.AddSingleton(config.Get<QueueWriterSettings>());
            services.AddSingleton(typeof(QueueWriter));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.OutputWriters
{
    public static class ServiceRegistration
    {
        public static void RegisterWriters(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(config.Get<CsvWriterSettings>());
            services.AddSingleton(typeof(CsvOutputWriter<>));

            services.AddSingleton(config.Get<SqlWriterSettings>());
            services.AddSingleton(typeof(SqlWriter<>));

            services.AddSingleton(config.Get<QueueWriterSettings>());
            services.AddSingleton(typeof(QueueWriter));
        }
    }
}

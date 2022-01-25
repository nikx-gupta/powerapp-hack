using System;
using System.Net.Http.Headers;
using ChangeTracking.Clients;
using ChangeTracking.Clients.Configuration;
using ChangeTracking.Clients.Dataverse;
using ChangeTracking.Clients.Sql;
using ChangeTracking.Core;
using ChangeTracking.Entities;
using Dataverse.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ChangeTracker.Poller
{
    public static class PollerServiceRegistration
    {
        public static void RegisterPoller(this IServiceCollection services, IConfiguration config)
        {
            services.RegisterSqlChangeTracking<AccountModel>(config.Get<SqlChangeTrackingSettings>());
            services.RegisterCsvFormatter(config.Get<CsvWriterSettings>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), typeof(PollerService<>)));

            // services.RegisterDataverseClient(config.Get<PowerAppTokenSettings>());
            // services.RegisterSqlWriters(config);
        }
    }
}
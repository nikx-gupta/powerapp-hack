using System;
using System.Net.Http.Headers;
using ChangeTracking.Clients;
using ChangeTracking.Clients.Configuration;
using ChangeTracking.Clients.Dataverse;
using ChangeTracking.Clients.Sql;
using ChangeTracking.Core;
using ChangeTracking.Core.Configuration;
using ChangeTracking.Core.Helpers;
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
            services.RegisterSqlChangeTracking<AccountModel>(config.GetSettings<SqlChangeTrackingSettings>());
            services.RegisterCsvFormatter(config.GetSettings<CsvWriterSettings>());
            services.AddSingleton(config.GetSettings<PollerServiceSettings>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(IHostedService), typeof(PollerService<AccountModel>)));

            // services.RegisterDataverseClient(config.Get<PowerAppTokenSettings>());
            // services.RegisterSqlWriters(config);
        }
    }
}
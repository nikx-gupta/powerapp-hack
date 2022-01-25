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

namespace ChangeTracker.Poller
{
    public static class PollerServiceRegistration
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(provider => config.Get<PowerAppClientSettings>());
            services.AddSingleton<TokenService>();
            services.AddTransient<DataverseClient>();
            services.AddTransient<TokenHandler>();

            services.AddHttpClient<DataverseClient>((svcProvider, client) =>
            {
                var settings = svcProvider.GetService<PowerAppClientSettings>();
                client.BaseAddress = new Uri(settings.DataverseUrl + "/api/data/v9.2/");
                client.Timeout = new TimeSpan(0, 2, 0);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();

            //services.AddScoped<IChangeTrackingClient<AccountModel>, DataverseChangeTrackingClient<AccountModel>>();
            services.AddScoped<IChangeTrackingClient<AccountModel>, SqlChangeTrackingClient<AccountModel>>();
            services.AddScoped(typeof(DataverseCrudWriter<>));

           services.RegisterSqlWriters(config);
           services.RegisterSqlChangeTracking(config);
        }
    }
}
using System.Net.Http.Headers;
using System.Transactions;
using Dataverse.Core.ChangeTracking;
using Dataverse.Core.Configuration;
using Dataverse.Core.Crud;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Dataverse.Core
{
    public static class DataverseServiceRegistration
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

            services.AddScoped(typeof(ChangeTrackingClient<>));
            services.AddScoped(typeof(CrudOperation<>));
        }
    }
}
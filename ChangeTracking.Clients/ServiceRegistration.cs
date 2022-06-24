using System;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using Azure.Storage.Queues;
using ChangeTracking.Clients.Cloud;
using ChangeTracking.Clients.Dataverse;
using ChangeTracking.Clients.Formatters;
using ChangeTracking.Clients.Sql;
using ChangeTracking.Core;
using ChangeTracking.Core.Configuration;
using ChangeTracking.Core.Store;
using ChangeTracking.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChangeTracking.Clients
{
    public static class ServiceRegistration
    {
        public static void RegisterDataverseClient(this IServiceCollection services, PowerAppTokenSettings config)
        {
            services.AddSingleton<TokenService>();
            services.AddTransient<DataverseServiceClient>();
            services.AddTransient<TokenHandler>();

            services.AddHttpClient<DataverseServiceClient>((svcProvider, client) =>
            {
                var settings = svcProvider.GetService<PowerAppTokenSettings>();
                client.BaseAddress = new Uri(settings.DataverseUrl + "/api/data/v9.2/");
                client.Timeout = new TimeSpan(0, 2, 0);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<TokenHandler>();
        }

        public static void RegisterSqlWriter(this IServiceCollection services, SqlWriterSettings config)
        {
            services.AddSingleton(config);
            services.AddTransient<IOutputWriter, SqlWriter>();
            services.TryAddSingleton<OutputWriterFactory>();
        }

        public static void RegisterQueueWriter(this IServiceCollection services, QueueClientConfig config)
        {
            services.AddSingleton<QueueClient>(provider => new QueueClient(config.QueueConnectionString, config.QueueName));
            services.AddTransient<IOutputWriter, QueueWriter>();
            services.TryAddSingleton<OutputWriterFactory>();
        }

        public static void RegisterCsvFormatter(this IServiceCollection services, CsvWriterSettings writerSettings)
        {
            services.AddSingleton(writerSettings);
            services.TryAddSingleton<OutputWriterFactory>();
            services.AddTransient<IOutputWriter, CsvFormatter>();
        }

        public static void RegisterDataverseChangeTracking<T>(this IServiceCollection services, PowerAppTokenSettings settings)
        {
            services.RegisterDataverseClient(settings);
            if (settings.IsFunctionApp)
            {
                services.AddTransient<IPowerAppTokenStore, BlobTokenStore>();
            }
            else
                services.AddTransient<IPowerAppTokenStore, InMemoryTokenStore>();

            services.AddScoped<IChangeTrackingClient<T>, DataverseChangeTrackingClient<T>>();
        }

        public static void RegisterSqlChangeTracking<T>(this IServiceCollection services, SqlChangeTrackingSettings settings) where T : ISqlTrackingEntity
        {
            services.AddSingleton<SqlConnection>(provider =>
                new SqlConnection(settings.SourceConnectionString));
            services.AddScoped<IChangeTrackingClient<T>, SqlChangeTrackingClient<T>>();
        }
    }
}

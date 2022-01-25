using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChangeTracking.Clients.Cloud;
using ChangeTracking.Clients.Configuration;
using ChangeTracking.Clients.Dataverse;
using ChangeTracking.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChangeTracker.Poller
{
    class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices((context, collection) =>
                {
                    collection.AddHostedService<DataSyncHost>();
                    PollerServiceRegistration.RegisterServices(collection, context.Configuration);
                })
                .ConfigureLogging((context, logBuilder) =>
                {
                    logBuilder.AddFilter((cat, level) =>
                    {
                        if (cat.StartsWith("System.Net.Http"))
                            return false;

                        return true;
                    });
                    logBuilder.AddConsole();
                })
                .ConfigureAppConfiguration((hostContext, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", true);
                })
                .Build();

            await host.RunAsync();
        }
    }

    public class DataSyncHost : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataSyncHost> _logger;

        public DataSyncHost(IServiceProvider serviceProvider, ILogger<DataSyncHost> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //var changeTracking = _serviceProvider.GetRequiredService<DataverseChangeTrackingClient<AccountModel>>();
            var changeTracking = _serviceProvider.GetRequiredService<DataverseChangeTrackingClient<AccountModel>>();
            var writerSettings = _serviceProvider.GetRequiredService<CsvWriterSettings>();
            var writer = _serviceProvider.GetRequiredService<QueueWriter>();

            var allRecords = await changeTracking.GetAllRecords();
            writer.WriteBatch(allRecords);

            while (true)
            {
                _logger.LogInformation($"Fetching Changes after Last Operation");
                var deltaRecords = await changeTracking.GetDeltaChanges();
                _logger.LogInformation($"Changed Record Count: {deltaRecords.Count}");
                _logger.LogInformation(JsonConvert.SerializeObject(deltaRecords, Formatting.Indented));
                _logger.LogInformation($"Next Change Token: {changeTracking.CurrentChangeToken}");
                if (deltaRecords.Any())
                {
                    writer.WriteBatch(deltaRecords.ToList());
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }

            //var crudRepo = _serviceProvider.GetRequiredService<DataverseCrudWriter<AccountModel>>();
            //var changeTracking = _serviceProvider.GetRequiredService<DataverseChangeTrackingClient<AccountModel>>();

            //await changeTracking.GetAllRecords();

            //var rec = await crudRepo.Insert(new AccountModel()
            //{
            //    AccountNo = Guid.NewGuid().ToString().Substring(0, 20),
            //    Name = DateTime.Now.ToString("dd-MM-yyyy HH:MM"),
            //    Telephone1 = "123456789"
            //});

            //string accountId = rec.AccountId;

            //await changeTracking.GetDeltaChanges();

            //await crudRepo.UpdateRecord(accountId, new AccountModel()
            //{
            //    AccountNo = Guid.NewGuid().ToString().Substring(0, 20),
            //    Name = DateTime.Now.ToString("dd-MM-yyyy HH:MM"),
            //    Telephone1 = "123456789"
            //});

            //await crudRepo.UpdateSingleProperty(accountId, "name", DateTime.Now.ToString("dd-MM-yyyy HH:MM"));

            //await crudRepo.ListRecords();

            //await crudRepo.GetRecord(accountId);

            //await crudRepo.DeleteProperty(accountId, "name");

            //await crudRepo.Delete(accountId);

            Console.ReadKey();

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
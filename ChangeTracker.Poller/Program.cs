using Dataverse.Core;
using Dataverse.Core.Crud;
using Dataverse.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PowerAppsConsole;

namespace ChangeTracker.Poller
{
    class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices((context, collection) =>
                {
                    collection.AddHostedService<PowerAppsHost>();
                    collection.RegisterServices(context.Configuration);
                })
                .ConfigureLogging((context, logBuilder) =>
                {
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

    public class PowerAppsHost : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public PowerAppsHost(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            //var crudRepo = _serviceProvider.GetRequiredService<CrudOperation<AccountModel>>();
            //var changeTracking = _serviceProvider.GetRequiredService<ChangeTrackingClient<AccountModel>>();

            //await changeTracking.GetChangeDelta();

            //var rec = await crudRepo.Insert(new AccountModel()
            //{
            //    AccountNo = Guid.NewGuid().ToString().Substring(0, 20),
            //    Name = DateTime.Now.ToString("dd-MM-yyyy HH:MM"),
            //    Telephone1 = "123456789"
            //});

            //string accountId = rec.AccountId;

            //await changeTracking.GetChangesAfterLastOperation();

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
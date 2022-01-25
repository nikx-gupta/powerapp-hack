using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChangeTracking.Clients;
using ChangeTracking.Clients.Cloud;
using ChangeTracking.Core.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChangeTracker.Poller;

public class PollerService<T> : IHostedService where T : class
{
    private readonly PollerServiceSettings _settings;
    private readonly IChangeTrackingClient<T> _trackingClient;
    private readonly OutputWriterFactory _writerFactory;
    private readonly ILogger<PollerService<T>> _logger;

    public PollerService(PollerServiceSettings settings, IChangeTrackingClient<T> trackingClient, OutputWriterFactory writerFactory, ILogger<PollerService<T>> logger)
    {
        _settings = settings;
        _trackingClient = trackingClient;
        _writerFactory = writerFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var allRecords = await _trackingClient.GetAllRecords();
        if (_settings.WriteAllRecordsOnStart)
        {
            var writer = _writerFactory.Instance();
            writer.WriteBatch(allRecords);
            _writerFactory.Flush();
        }

        while (true)
        {
            _logger.LogInformation($"Fetching Changes after Last Operation");
            var deltaRecords = await _trackingClient.GetDeltaChanges();
            _logger.LogInformation($"Changed Record Count: {deltaRecords.Count}");
            _logger.LogInformation(JsonConvert.SerializeObject(deltaRecords, Formatting.Indented));
            if (deltaRecords.Any())
            {
                var writer = _writerFactory.Instance();
                writer.WriteBatch(deltaRecords.ToList());
                _writerFactory.Flush();
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
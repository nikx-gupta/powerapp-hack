using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChangeTracking.Clients.Formatters;
using ChangeTracking.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.Listener
{
    public class ClientHost : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ClientHost> _logger;

        public ClientHost(IServiceProvider serviceProvider, ILogger<ClientHost> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listening For Update Tracking Messages");

            var queueClient = _serviceProvider.GetService<Azure.Storage.Queues.QueueClient>();

            while (true)
            {
                var msg = await queueClient.ReceiveMessagesAsync(cancellationToken);
                if (msg.Value.Length > 0)
                {
                    using var writer = _serviceProvider.GetService<CsvFormatter<AccountModel>>();
                    foreach (var queueMessage in msg.Value)
                    {
                        writer.WriteBatch(queueMessage.Body.ToObjectFromJson<List<AccountModel>>());
                        await queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, cancellationToken);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using ChangeTracking.Clients;
using ChangeTracking.Clients.Formatters;
using ChangeTracking.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.Listener
{
    public class ClientHost : IHostedService
    {
        private readonly QueueClient _queueClient;
        private readonly OutputWriterFactory _writerFactory;
        private readonly ILogger<ClientHost> _logger;

        public ClientHost(QueueClient queueClient, OutputWriterFactory writerFactory, ILogger<ClientHost> logger)
        {
            _queueClient = queueClient;
            _writerFactory = writerFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listening For Update Tracking Messages");

            while (true)
            {
                var msg = await _queueClient.ReceiveMessagesAsync(cancellationToken);
                if (msg.Value.Length > 0)
                {
                    var writer = _writerFactory.Instance();
                    foreach (var queueMessage in msg.Value)
                    {
                        writer.WriteBatch(queueMessage.Body.ToObjectFromJson<List<AccountModel>>());
                        await _queueClient.DeleteMessageAsync(queueMessage.MessageId, queueMessage.PopReceipt, cancellationToken);
                    }

                    _writerFactory.Flush();
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
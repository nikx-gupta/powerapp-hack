using ChangeTracker.Listener.Library;
using Dataverse.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.Listener
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((context, collection) =>
                {
                    collection.AddHostedService<ClientHost>();
                    collection.RegisterServices(context.Configuration);
                })
                .ConfigureLogging((context, logBuilder) =>
                {
                    logBuilder.AddConfiguration(context.Configuration);
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
                    using var writer = new CsvOutputWriter<AccountModel>();
                    foreach (var queueMessage in msg.Value)
                    {
                        writer.Write(queueMessage.Body.ToObjectFromJson<AccountModel>());
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
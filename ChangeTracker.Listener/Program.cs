using System.Threading.Tasks;
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
}
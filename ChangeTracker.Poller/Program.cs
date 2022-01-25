using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.Poller
{
    class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureServices((context, collection) =>
                {
                    collection.RegisterPoller(context.Configuration);
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
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

[assembly: WebJobsStartup(typeof(PowerApps.ChangeListenerFunc.Startup))]
namespace PowerApps.ChangeListenerFunc
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            //builder.AddAzureStorageCoreServices();
            //var token = new Azure.Identity.AzureCliCredential();

            //var t = token.GetToken(new TokenRequestContext(new[]
            //{
            //    "https://org90a5ba8a.api.crm8.dynamics.com"
            //}), CancellationToken.None);

            //Console.WriteLine(t.Token);

            builder.Services.AddTransient<CloudStorageAccount>((provider) =>
            {
                var config = provider.GetService<IConfiguration>();
                var connString = config.GetWebJobsConnectionString("Storage");
                return connString.Contains("UseDevelopmentStorage")
                    ? CloudStorageAccount.DevelopmentStorageAccount
                    : CloudStorageAccount.Parse(connString);
            });
            builder.Services.AddTransient<BlobContainerClient>((provider) =>
            {
                var config = provider.GetService<IConfiguration>();
                var strAcct = provider.GetService<CloudStorageAccount>();
                var containerName  = config.GetValue<string>()
                return new BlobContainerClient(strAcct.BlobEndpoint);
            });

        }
    }
}

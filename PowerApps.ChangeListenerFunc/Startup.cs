using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

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
            //Azure.Storage.
            //var storeCred = new StorageCredentials((TokenCredential)ced);
            //builder.Services.AddTransient<CloudStorageAccount>((provider) =>
            //{
            //    var config = provider.GetService<IConfiguration>();
            //    var connString = config.GetWebJobsConnectionString("Storage");
            //    return connString.Contains("UseDevelopmentStorage")
            //        ? CloudStorageAccount.DevelopmentStorageAccount
            //        : new CloudStorageAccount()
            //});
            var containerUri = "https://pocnikxdataversestore.blob.core.windows.net/test";
            builder.Services.AddTransient<BlobContainerClient>((provider) =>
            {
                var config = provider.GetService<IConfiguration>();
                return new BlobContainerClient(new Uri(containerUri), new DefaultAzureCredential());
            });

        }
    }
}

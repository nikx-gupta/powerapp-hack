using System;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace PowerApps.ChangeListenerFunc
{
    public class ChangeListener
    {
        private readonly IConfiguration _config;

        public ChangeListener(BlobContainerClient client, IConfiguration config)
        {
            _config = config;
        }

        [FunctionName(nameof(ChangeListener))]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"{_config["DataverseUrl"]}");
            //log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        }
    }
}

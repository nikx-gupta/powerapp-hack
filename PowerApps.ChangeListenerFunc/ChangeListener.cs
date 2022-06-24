using System;
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
        private readonly CloudStorageAccount _storeAccount;

        public ChangeListener(CloudStorageAccount storeAccount)
        {
            _storeAccount = storeAccount;
            
        }

        [FunctionName("Function1")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        }
    }
}

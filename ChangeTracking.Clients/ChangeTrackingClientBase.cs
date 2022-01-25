using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace ChangeTracking.Clients
{
    public class ChangeTrackingClientBase
    {
        protected readonly ILogger _logger;
        public ChangeTrackingClientBase(ILogger logger)
        {
            _logger = logger;
        }

        public bool HasFailure(HttpResponseMessage msg)
        {
            if (!msg.IsSuccessStatusCode)
            {
                _logger.LogInformation("Error");
                _logger.LogInformation(msg.Content.ReadAsStringAsync().Result);
                return true;
            }

            return false;
        }
    }
}


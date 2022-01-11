using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dataverse.Core.Configuration
{
    public class PowerAppClientSettings
    {
        public string DataverseUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TenantId { get; set; }
    }
}

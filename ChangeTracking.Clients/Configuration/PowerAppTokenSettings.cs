using System;
using ChangeTracking.Core.Configuration;

namespace ChangeTracking.Clients.Configuration
{
    public class PowerAppTokenSettings : AzureTokenSettings
    {
        public string DataverseUrl { get; set; }
        public override string ResourceUrl => DataverseUrl;
    }

    public class SqlChangeTrackingSettings
    {
        public string SourceConnectionString { get; set; }
    }
}

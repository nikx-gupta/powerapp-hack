namespace ChangeTracking.Core.Configuration
{
    public class AzureTokenSettings
    {
        public virtual string ResourceUrl { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string TenantId { get; set; }
    }

    public class PollerServiceSettings
    {
        public bool WriteAllRecordsOnStart { get; set; }
    }
}

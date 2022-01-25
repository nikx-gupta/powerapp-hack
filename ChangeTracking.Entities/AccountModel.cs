using Newtonsoft.Json;

namespace ChangeTracking.Entities
{
    [DataverseTable(Name = "accounts")]
    public class AccountModel : BaseEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("accountnumber")]
        public string AccountNo { get; set; }
        [JsonProperty("telephone1")]
        public string Telephone1 { get; set; }
        [JsonProperty("fax")]
        public string Fax { get; set; }
        [JsonProperty("accountid")]
        public string AccountId { get; set; }
    }

    public class BaseEntity
    {
        //[JsonProperty("@odata.context")]
        //public string Context { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}

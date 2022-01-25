using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChangeTracking.Entities
{
    public class DataverseResponse<T>
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        [JsonProperty("@odata.deltaLink")]
        public string DeltaLink { get; set; }

        [JsonProperty("value")]
        public List<T> Records { get; set; }
    }
}

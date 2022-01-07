using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAppsConsole.Models
{
    public class AccountModel
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
}

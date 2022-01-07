using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerAppsConsole.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerAppsConsole
{
    public class ChangeTrackingOperation<T>
    {
        private readonly Regex regexToken = new Regex("\\$deltatoken=(?<token>.+)", RegexOptions.Compiled);
        private readonly HttpClient client;

        public string TableName { get; }

        public string CurentChangeLink { get; private set; }
        public string CurrentChangeToken { get; private set; }

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public ChangeTrackingOperation(ServiceClient client, string tableName)
        {
            this.client = client.CreateClient().Result;
            TableName = tableName;
        }

        public async Task GetChangeDelta()
        {
            ClearHeaders();
            client.DefaultRequestHeaders.Add("PREFER", "odata.track-changes");
            var result = await client.GetAsync(TableName);
            var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
            CurentChangeLink = record.DeltaLink;
            CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;

            Console.WriteLine($"Current Change Token: {CurrentChangeToken}");
        }

        public async Task GetChangesAfterLastOperation()
        {
            ClearHeaders();
            client.DefaultRequestHeaders.Add("PREFER", "odata.track-changes");
            var result = await client.GetAsync($"{TableName}?$deltatoken={CurrentChangeToken}");

            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
                Console.WriteLine(JsonConvert.SerializeObject(record.Records, Formatting.Indented));

                CurentChangeLink = record.DeltaLink;
                CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;

                Console.WriteLine($"\nCurrent Change Token: {CurrentChangeToken}");
            }
        }

        public bool HasFailure(HttpResponseMessage msg)
        {
            if (!msg.IsSuccessStatusCode)
            {
                Console.WriteLine("Error");
                Console.WriteLine(msg.Content.ReadAsStringAsync().Result);
                return true;
            }

            return false;
        }

        private void ClearHeaders()
        {
            if (client.DefaultRequestHeaders.Contains("If-Match"))
            {
                client.DefaultRequestHeaders.Remove("If-Match");
            }
            if (client.DefaultRequestHeaders.Contains("PREFER"))
            {
                client.DefaultRequestHeaders.Remove("PREFER");
            }
        }
    }
}

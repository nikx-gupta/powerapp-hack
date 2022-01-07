using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PowerAppsConsole
{
    public class CrudOperation<T>
    {
        private readonly HttpClient client;

        public string TableName { get; }
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public CrudOperation(HttpClient client, string tableName)
        {
            this.client = client;
            TableName = tableName;

        }

        public async Task UpdateRecord(string primaryKey, T model)
        {
            ClearHeaders();
            client.DefaultRequestHeaders.Add("PREFER", "return=representation");
            client.DefaultRequestHeaders.Add("If-Match", "*");
            var result = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{TableName}({primaryKey})")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model, settings), Encoding.UTF8, "application/json")
            });

            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                Console.WriteLine("Updated Successfully");
                Console.WriteLine(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task ListRecords()
        {
            ClearHeaders();
            var result = await client.GetAsync(TableName);
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
                Console.WriteLine(JsonConvert.SerializeObject(record.Records, Formatting.Indented));
            }
        }

        public async Task GetRecord(string primaryKey)
        {
            ClearHeaders();
            var result = await client.GetAsync($"{TableName}({primaryKey})");
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                Console.WriteLine(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task UpdateSingleProperty(string primaryKey, string propertyName, string propertyValue)
        {
            ClearHeaders();
            client.DefaultRequestHeaders.Add("PREFER", "return=representation");
            client.DefaultRequestHeaders.Add("If-Match", "*");
            var result = await client.PutAsync($"{TableName}({primaryKey})/{propertyName}", new StringContent(JsonConvert.SerializeObject(new
            {
                value = propertyValue
            }, settings), Encoding.UTF8, "application/json"));

            if (!HasFailure(result))
            {
                Console.WriteLine("Updated Successfully");
                await GetRecord(primaryKey);
            }
        }

        public async Task<T> Insert(T model)
        {
            ClearHeaders();
            client.DefaultRequestHeaders.Add("PREFER", "return=representation");

            var result = await client.PostAsync(TableName, new StringContent(JsonConvert.SerializeObject(model, settings), Encoding.UTF8, "application/json"));
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                Console.WriteLine("Insertion Success");
                Console.WriteLine(JsonConvert.SerializeObject(record, Formatting.Indented));

                return record;
            }

            return default(T);
        }

        public async Task Delete(string primaryKey)
        {
            ClearHeaders();
            var result = await client.DeleteAsync($"{TableName}({primaryKey})");
            if (!HasFailure(result) && result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine($"Deleted Record with Key :{primaryKey}");
            }
        }

        public async Task DeleteProperty(string primaryKey, string propertyName)
        {
            ClearHeaders();
            var result = await client.DeleteAsync($"{TableName}({primaryKey})/{propertyName}");
            if (!HasFailure(result) && result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine($"Deleted Record with Key :{primaryKey}");
                await GetRecord(primaryKey);
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

    public class DataverseResponse<T>
    {
        [JsonProperty("value")]
        public List<T> Records { get; set; }
    }
}

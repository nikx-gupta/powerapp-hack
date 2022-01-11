using System.Text;
using Dataverse.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dataverse.Core.Crud
{
  

    public class CrudOperation<T>
    {
        private readonly DataverseClient _client;
        private readonly ILogger<CrudOperation<T>> _logger;
        public string TableName { get; }

        readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public CrudOperation(DataverseClient client, ILogger<CrudOperation<T>> logger)
        {
            _client = client;
            _logger = logger;
            var tableAttr = (DataverseTable)Attribute.GetCustomAttribute(typeof(T), typeof(DataverseTable))!;
            if (tableAttr == null)
            {
                throw new Exception($"DataverseTable Attribute is required on Model {typeof(T).Name}");
            }

            TableName = tableAttr.Name;
        }

        public async Task UpdateRecord(string primaryKey, T model)
        {
            _client.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "return=representation");
                headers.Add("If-Match", "*");
            });

            var result = await _client.Client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{TableName}({primaryKey})")
            {
                Content = new StringContent(JsonConvert.SerializeObject(model, settings), Encoding.UTF8, "application/json")
            });

            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation("Updated Successfully");
                _logger.LogInformation(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task ListRecords()
        {
            var result = await _client.Client.GetAsync(TableName);
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task GetRecord(string primaryKey)
        {
            var result = await _client.Client.GetAsync($"{TableName}({primaryKey})");
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task UpdateSingleProperty(string primaryKey, string propertyName, string propertyValue)
        {
            _client.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "return=representation");
                headers.Add("If-Match", "*");
            });

            var result = await _client.Client.PutAsync($"{TableName}({primaryKey})/{propertyName}", new StringContent(JsonConvert.SerializeObject(new
            {
                value = propertyValue
            }, settings), Encoding.UTF8, "application/json"));

            if (!HasFailure(result))
            {
                _logger.LogInformation("Updated Successfully");
                await GetRecord(primaryKey);
            }
        }

        public async Task<T> Insert(T model)
        {
            _client.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "return=representation");
            });

            var result = await _client.Client.PostAsync(TableName, new StringContent(JsonConvert.SerializeObject(model, settings), Encoding.UTF8, "application/json"));
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation("Insertion Success");
                _logger.LogInformation(JsonConvert.SerializeObject(record, Formatting.Indented));

                return record;
            }

            return default(T);
        }

        public async Task Delete(string primaryKey)
        {
            var result = await _client.Client.DeleteAsync($"{TableName}({primaryKey})");
            if (!HasFailure(result) && result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogInformation($"Deleted Record with Key :{primaryKey}");
            }
        }

        public async Task DeleteProperty(string primaryKey, string propertyName)
        {
            var result = await _client.Client.DeleteAsync($"{TableName}({primaryKey})/{propertyName}");
            if (!HasFailure(result) && result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogInformation($"Deleted Record with Key :{primaryKey}");
                await GetRecord(primaryKey);
            }
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

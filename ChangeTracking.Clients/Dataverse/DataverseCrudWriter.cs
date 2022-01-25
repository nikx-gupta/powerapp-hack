using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChangeTracking.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChangeTracking.Clients.Dataverse
{
    public class DataverseCrudWriter<T>
    {
        private readonly DataverseServiceClient _serviceClient;
        private readonly ILogger<DataverseCrudWriter<T>> _logger;
        public string TableName { get; }

        readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public DataverseCrudWriter(DataverseServiceClient serviceClient, ILogger<DataverseCrudWriter<T>> logger)
        {
            _serviceClient = serviceClient;
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
            _serviceClient.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "return=representation");
                headers.Add("If-Match", "*");
            });

            var result = await _serviceClient.Client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"{TableName}({primaryKey})")
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
            var result = await _serviceClient.Client.GetAsync(TableName);
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task GetRecord(string primaryKey)
        {
            var result = await _serviceClient.Client.GetAsync($"{TableName}({primaryKey})");
            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<T>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation(JsonConvert.SerializeObject(record, Formatting.Indented));
            }
        }

        public async Task UpdateSingleProperty(string primaryKey, string propertyName, string propertyValue)
        {
            _serviceClient.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "return=representation");
                headers.Add("If-Match", "*");
            });

            var result = await _serviceClient.Client.PutAsync($"{TableName}({primaryKey})/{propertyName}", new StringContent(JsonConvert.SerializeObject(new
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
            _serviceClient.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "return=representation");
            });

            var result = await _serviceClient.Client.PostAsync(TableName, new StringContent(JsonConvert.SerializeObject(model, settings), Encoding.UTF8, "application/json"));
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
            var result = await _serviceClient.Client.DeleteAsync($"{TableName}({primaryKey})");
            if (!HasFailure(result) && result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogInformation($"Deleted Record with Key :{primaryKey}");
            }
        }

        public async Task DeleteProperty(string primaryKey, string propertyName)
        {
            var result = await _serviceClient.Client.DeleteAsync($"{TableName}({primaryKey})/{propertyName}");
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

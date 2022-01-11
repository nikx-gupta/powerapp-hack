using System.Text.RegularExpressions;
using Dataverse.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Dataverse.Core.ChangeTracking
{
    /// <summary>
    /// Change Tracking for Respective Entity/Table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ChangeTrackingClient<T>
    {
        private readonly Regex regexToken = new Regex("\\$deltatoken=(?<token>.+)", RegexOptions.Compiled);
        private readonly DataverseClient client;
        private readonly ILogger<ChangeTrackingClient<T>> _logger;

        public string TableName { get; }
        public string CurentChangeLink { get; private set; }
        public string CurrentChangeToken { get; private set; }

        public ChangeTrackingClient(DataverseClient client, ILogger<ChangeTrackingClient<T>> logger)
        {
            this.client = client;
            _logger = logger;
            var tableAttr = (DataverseTable) Attribute.GetCustomAttribute(typeof(T), typeof(DataverseTable))!;
            if (tableAttr == null)
            {
                throw new Exception($"DataverseTable Attribute is required on Model {typeof(T).Name}");
            }

            TableName = tableAttr.Name;
        }

        public async Task GetAllRecords()
        {
            client.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "odata.track-changes");
            });
            
            var result = await client.Client.GetAsync(TableName);
            var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
            CurentChangeLink = record.DeltaLink;
            CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;

            _logger.LogInformation($"Current Change Token: {CurrentChangeToken}");
        }

        public async Task<List<T>> GetChangesAfterLastOperation()
        {
            client.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "odata.track-changes");
            });

            var result = await client.Client.GetAsync($"{TableName}?$deltatoken={CurrentChangeToken}");

            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
                
                CurentChangeLink = record.DeltaLink;
                CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;

                return record.Records;
            }

            return new List<T>(0);
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

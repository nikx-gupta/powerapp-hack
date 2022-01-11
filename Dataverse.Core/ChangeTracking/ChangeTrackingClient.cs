using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dataverse.Core;
using Dataverse.Core.Crud;
using Dataverse.Entities;
using Microsoft.Extensions.Logging;

namespace PowerAppsConsole
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

        public async Task GetChangeDelta()
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

        public async Task GetChangesAfterLastOperation()
        {
            client.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "odata.track-changes");
            });

            var result = await client.Client.GetAsync($"{TableName}?$deltatoken={CurrentChangeToken}");

            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
                _logger.LogInformation($"Changes after Last Operation");
                _logger.LogInformation(JsonConvert.SerializeObject(record.Records, Formatting.Indented));

                CurentChangeLink = record.DeltaLink;
                CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;

                _logger.LogInformation($"\nCurrent Change Token: {CurrentChangeToken}");
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

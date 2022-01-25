using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ChangeTracking.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChangeTracking.Clients.Dataverse
{
    /// <summary>
    /// Change Tracking for Respective Entity/Table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataverseChangeTrackingClient<T> : ChangeTrackingClientBase, IChangeTrackingClient<T>
    {
        private readonly Regex regexToken = new Regex("\\$deltatoken=(?<token>.+)", RegexOptions.Compiled);
        private readonly DataverseServiceClient _serviceClient;

        public string TableName { get; }
        public string CurentChangeLink { get; private set; }
        public string CurrentChangeToken { get; private set; }

        public DataverseChangeTrackingClient(DataverseServiceClient serviceClient, ILogger<DataverseChangeTrackingClient<T>> logger) : base(logger)
        {
            this._serviceClient = serviceClient;
            var tableAttr = (DataverseTable)Attribute.GetCustomAttribute(typeof(T), typeof(DataverseTable))!;
            if (tableAttr == null)
            {
                throw new Exception($"DataverseTable Attribute is required on Model {typeof(T).Name}");
            }

            TableName = tableAttr.Name;
        }

        public async Task<List<T>> GetAllRecords()
        {
            _serviceClient.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "odata.track-changes");
            });

            var result = await _serviceClient.Client.GetAsync(TableName);
            var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());
            CurentChangeLink = record.DeltaLink;
            CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;

            _logger.LogInformation($"Current Change Token: {CurrentChangeToken}");

            return record.Records;
        }

        public async Task<List<T>> GetDeltaChanges()
        {
            _serviceClient.AddHeaders((headers) =>
            {
                headers.Add("PREFER", "odata.track-changes");
            });

            var result = await _serviceClient.Client.GetAsync($"{TableName}?$deltatoken={CurrentChangeToken}");

            if (!HasFailure(result))
            {
                var record = JsonConvert.DeserializeObject<DataverseResponse<T>>(await result.Content.ReadAsStringAsync());

                CurentChangeLink = record.DeltaLink;
                CurrentChangeToken = regexToken.Match(CurentChangeLink)?.Groups["token"].Value;
                _logger.LogInformation($"Next Change Token: {CurrentChangeToken}");

                return record.Records;
            }

            return new List<T>(0);
        }
    }
}

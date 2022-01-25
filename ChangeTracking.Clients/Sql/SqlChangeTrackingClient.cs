using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracking.Clients.Dataverse;
using ChangeTracking.Core.Helpers;
using ChangeTracking.Entities;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;

namespace ChangeTracking.Clients.Sql
{
    /// <summary>
    /// Change Tracking for Respective Entity/Table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlChangeTrackingClient<T> : ChangeTrackingClientBase, IChangeTrackingClient<T> where T : ISqlTrackingEntity
    {
        public string TableName { get; }
        public string ModifiedDateColumnName { get; }

        public DateTime LastModifiedDate { get; private set; }
        private SqlConnection _reader;

        public SqlChangeTrackingClient(SqlConnection connection, ILogger<SqlChangeTrackingClient<T>> logger) : base(logger)
        {
            _reader = connection;
            var tableAttr = AttributeHelper.GetAttributeName<DataverseTable, T>(isRequired: true);
            var modifiedDate = AttributeHelper.GetAttributeName<ChangeTrackingModifiedDate, T>(isRequired: true);

            TableName = tableAttr.Name;
            ModifiedDateColumnName = modifiedDate.Name;
            LastModifiedDate = DateTime.Today.AddDays(-30);
;        }

        public async Task<List<T>> GetAllRecords()
        {
            return new List<T>();
        }

        public async Task<List<T>> GetDeltaChanges()
        {
            var results = (await _reader.QueryAsync<T>($"SELECT * FROM {TableName} WHERE {ModifiedDateColumnName} > @ModifiedDate ORDER BY {ModifiedDateColumnName}", new { ModifiedDate = LastModifiedDate })).ToList();
            if (results.Any())
            {
                LastModifiedDate = results.Last().LastModifiedDate;
            }

            return results;
        }
    }
}
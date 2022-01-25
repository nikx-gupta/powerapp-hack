using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ChangeTracking.Clients.Dataverse;
using ChangeTracking.Core.Helpers;
using ChangeTracking.Entities;
using Microsoft.Extensions.Logging;

namespace ChangeTracking.Clients.Sql
{
    /// <summary>
    /// Change Tracking for Respective Entity/Table
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlChangeTrackingClient<T>
    {
        private readonly ILogger<SqlChangeTrackingClient<T>> _logger;

        public string TableName { get; }
        public string ModifiedDateColumnName { get; }

        public DateTime LastModifiedDate { get; private set; }
    
        private SqlConnection _reader;

        public SqlChangeTrackingClient(SqlConnectionSettings settings, ILogger<SqlChangeTrackingClient<T>> logger)
        {
            _logger = logger;
            var tableAttr = AttributeHelper.GetAttributeName<DataverseTable, T>(isRequired: true);
            var modifiedDate = AttributeHelper.GetAttributeName<ChangeTrackingModifiedDate, T>(isRequired: true);

            TableName = tableAttr.Name;
            ModifiedDateColumnName = modifiedDate.Name;

            _reader = new SqlConnection(settings.SourceConnectionString);
        }

        public async Task<List<T>> GetAllRecords()
        {
            return new List<T>();
        }

        public async Task<List<T>> GetDeltaChanges()
        {
            return null;
        }
    }
}
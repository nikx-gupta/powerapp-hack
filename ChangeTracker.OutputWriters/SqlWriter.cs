using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using Dapper.Contrib.Extensions;
using Dataverse.Entities;
using Z.Dapper.Plus;

namespace ChangeTracker.OutputWriters
{
    public class SqlWriter<T> : IDisposable where T : class
    {
        private SqlConnection _writer;
        public SqlWriter(SqlWriterSettings settings)
        {
            var tblName = ((DataverseTable)Attribute.GetCustomAttribute(typeof(T), typeof(DataverseTable)))?.Name;
            DapperPlusManager.Entity<T>().Table(tblName);

            _writer = new SqlConnection(settings.ConnectionString);
        }

        public void Write(T objData)
        {
            _writer.Open();
            _writer.Insert(objData);
        }

        public void WriteBatch(List<T> data)
        {
            _writer.Open();
            _writer.BulkInsert(data);
        }

        public void Dispose()
        {
            _writer.Close();
            _writer.Dispose();
        }
    }
}

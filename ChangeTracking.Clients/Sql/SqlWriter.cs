using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ChangeTracking.Clients.Configuration;
using ChangeTracking.Entities;
using Dapper.Contrib.Extensions;
using Z.Dapper.Plus;

namespace ChangeTracking.Clients.Sql
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

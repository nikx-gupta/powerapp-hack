using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ChangeTracking.Clients.Configuration;
using ChangeTracking.Core.Helpers;
using ChangeTracking.Entities;
using Dapper.Contrib.Extensions;
using Z.Dapper.Plus;

namespace ChangeTracking.Clients.Sql
{
    public class SqlWriter : IOutputWriter
    {
        private readonly SqlConnection _writer;
        public SqlWriter(SqlConnection settings)
        {
            _writer = settings;
        }

        public void Write<T>(T objData) where T : class
        {
            var tblName = AttributeHelper.GetAttributeName<DataverseTable, T>(true).Name;
            DapperPlusManager.Entity<T>().Table(tblName);

            _writer.Open();
            _writer.Insert(objData);
        }

        public void WriteBatch<T>(List<T> data) where T : class
        {
            var tblName = AttributeHelper.GetAttributeName<DataverseTable, T>(true).Name;
            DapperPlusManager.Entity<T>().Table(tblName);
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

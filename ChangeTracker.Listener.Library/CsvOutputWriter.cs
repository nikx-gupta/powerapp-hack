using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace ChangeTracker.Listener.Library
{
    public class CsvOutputWriter<T> : IDisposable
    {
        private CsvWriter _writer;
        public CsvOutputWriter()
        {
            var fileName = $"Delta_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm")}.csv";
            StreamWriter sw;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            if (File.Exists(fileName))
            {
                sw = new StreamWriter(File.Open(fileName, FileMode.Append));
                config.HasHeaderRecord = false;
            }
            else
            {
                sw = new StreamWriter(File.Create(fileName));
            }

            _writer = new CsvWriter(sw, config);
        }

        public void Write(T objData)
        {
            _writer.WriteRecord(objData);
        }

        public void WriteBatch(List<T> data)
        {
            _writer.WriteRecords(data);
        }

        public void Dispose()
        {
            _writer.Flush();
            _writer.Dispose();
        }
    }
}

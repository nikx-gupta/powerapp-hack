using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace ChangeTracker.Listener.Library
{
    public class CsvOutputWriter<T> : IDisposable
    {
        private CsvWriter _writer;
        private bool _existingFile = false;
        public CsvOutputWriter()
        {
            var fileName = $"Delta_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm")}.csv";
            StreamWriter sw;
            if (File.Exists(fileName))
            {
                sw = new StreamWriter(fileName, Encoding.UTF8, new FileStreamOptions()
                {
                    Mode = FileMode.Append
                });
                _existingFile = true;
            }
            else
            {
                sw = new StreamWriter(fileName);
            }

            _writer = new CsvWriter(sw, CultureInfo.InvariantCulture, false);
        }

        public void Write(T objData)
        {
            if (!_existingFile)
                _writer.WriteHeader<T>();

            _writer.WriteRecord(objData);
        }

        public void WriteBatch(List<T> data)
        {
            if (!_existingFile)
                _writer.WriteHeader<T>();

            _writer.WriteRecords(data);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}

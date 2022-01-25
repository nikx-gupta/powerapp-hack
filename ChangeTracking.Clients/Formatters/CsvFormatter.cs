using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ChangeTracking.Clients.Cloud;
using ChangeTracking.Clients.Configuration;
using CsvHelper;
using CsvHelper.Configuration;

namespace ChangeTracking.Clients.Formatters
{
    public class CsvFormatter : IOutputWriter
    {
        private CsvWriter _writer;
        public CsvFormatter(CsvWriterSettings settings)
        {
            var fileName = $"Delta_{DateTime.Now.ToString("yyyy-MM-dd_hh_mm")}.csv";
            if (!Directory.Exists(settings.OutputPath))
                Directory.CreateDirectory(settings.OutputPath);

            var filePath = Path.Combine(settings.OutputPath, fileName);
            StreamWriter sw;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);

            if (File.Exists(filePath))
            {
                sw = new StreamWriter(File.Open(filePath, FileMode.Append));
                config.HasHeaderRecord = false;
            }
            else
            {
                sw = new StreamWriter(File.Create(filePath));
            }

            _writer = new CsvWriter(sw, config);
        }

        public void Write<T>(T objData) where T : class
        {
            _writer.WriteRecord(objData);
        }

        public void WriteBatch<T>(List<T> data) where T : class
        {
            _writer.WriteRecords(data);
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}

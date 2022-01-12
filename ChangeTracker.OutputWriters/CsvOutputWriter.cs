﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace ChangeTracker.OutputWriters
{
    public class CsvOutputWriter<T> : IDisposable
    {
        private CsvWriter _writer;
        public CsvOutputWriter(CsvWriterSettings settings)
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
            _writer.Dispose();
        }
    }
}

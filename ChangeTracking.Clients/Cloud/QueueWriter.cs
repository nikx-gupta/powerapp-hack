using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Queues;
using ChangeTracking.Clients.Configuration;

namespace ChangeTracking.Clients.Cloud
{
    public class QueueWriter : IDisposable
    {
        private QueueClient _writer;
        public QueueWriter(QueueWriterSettings settings)
        {
            _writer = new QueueClient(settings.QueueConnectionString, settings.QueueName);
        }

        public void Write<T>(T objData)
        {
            _writer.SendMessage(BinaryData.FromObjectAsJson(objData, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }));
        }

        public void WriteBatch<T>(List<T> data)
        {
            _writer.SendMessage(BinaryData.FromObjectAsJson(data, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }));
        }

        public void Dispose()
        {
            
        }
    }
}

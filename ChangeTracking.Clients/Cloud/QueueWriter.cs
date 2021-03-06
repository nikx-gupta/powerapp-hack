using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Queues;

namespace ChangeTracking.Clients.Cloud
{
    public class QueueWriter : IOutputWriter
    {
        private QueueClient _writer;
        public QueueWriter(QueueClient queueClient)
        {
            _writer = queueClient;
        }

        public void Write<T>(T objData) where T : class
        {
            _writer.SendMessage(BinaryData.FromObjectAsJson(objData, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }));
        }

        public void WriteBatch<T>(List<T> data) where T : class
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

namespace ChangeTracking.Clients.Configuration
{
    public class CsvWriterSettings
    {
        public string OutputPath { get; set; }
    }

    public class SqlWriterSettings
    {
        public string ConnectionString { get; set; }
    }

    public class QueueWriterSettings
    {
        public string QueueConnectionString { get; set; }
        public string QueueName{ get; set; }
    }
}
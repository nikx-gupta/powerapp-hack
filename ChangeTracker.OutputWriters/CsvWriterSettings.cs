namespace ChangeTracker.OutputWriters;

public class CsvWriterSettings
{
    public string OutputPath { get; set; }
}

public class SqlWriterSettings
{
    public string ConnectionString { get; set; }
}
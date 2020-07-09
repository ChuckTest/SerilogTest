using System;
using System.Diagnostics;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace SerilogTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Guid guid = Guid.NewGuid();

                var loggerConfiguration = new LoggerConfiguration();
                loggerConfiguration.ReadFrom.AppSettings();
                Log.Logger = loggerConfiguration.CreateLogger();

                var loggerConfiguration2 = new LoggerConfiguration()
                    .WriteTo.Logger(Log.Logger)
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions()
                    {
                        FailureCallback = FailureCallback,
                        EmitEventFailure = EmitEventFailureHandling.RaiseCallback
                    });
                Log.Logger = loggerConfiguration2.CreateLogger();
                for (int i = 0; i < 1000; i++)
                {
                    Log.Information($"{guid}, {i}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                }

                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void FailureCallback(LogEvent e)
        {
            Console.WriteLine($"Unable to submit event {e.MessageTemplate},{Environment.NewLine}{e.Exception}");
            if (e.Exception != null)
            {
                var content = $"{e.Exception}";
                SerilogEventLog lisaEventLog = new SerilogEventLog();
                lisaEventLog.WriteEntry(content, EventLogEntryType.Error);
            }
        }
    }
}

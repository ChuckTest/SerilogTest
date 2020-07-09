using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace SerilogTest
{
    internal class Program
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        static void Main(string[] args)
        {
            try
            {
                Guid guid = Guid.NewGuid();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .CreateLogger();

                //for (int i = 0; i < 1000; i++)
                //{
                //    Log.Information($"{guid}, {i}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                //}
                Log.Information($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
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

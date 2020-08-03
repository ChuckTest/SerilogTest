using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace SerilogTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                SelfLog.Enable(SelfLogHandler);
                Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var loggerConfiguration = new LoggerConfiguration().ReadFrom.Configuration(configuration);
                var logger = loggerConfiguration.CreateLogger();
                Log.Logger = logger;

                Guid guid = Guid.NewGuid();
                Log.Information($"{guid}, this is a information log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.Error($"{guid}, this is an error log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.Debug($"{guid}, this is a debug log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static readonly SerilogEventLog serilogEventLog = new SerilogEventLog();
        static void SelfLogHandler(string log)
        {
            serilogEventLog.WriteEntry(log, EventLogEntryType.Error);
        }
    }
}

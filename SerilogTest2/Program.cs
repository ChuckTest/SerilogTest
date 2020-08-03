using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Debugging;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace SerilogTest2
{
    class Program
    {
        static string GetConfigPath()
        {
            const string componentName = "serilog";
            const string fileExtension = "config";
            var fileName = $"{componentName}.{fileExtension}";
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            var configFile =
                new FileInfo(Path.Combine(baseDirectory,
                    AppDomain.CurrentDomain.FriendlyName + $".{fileName}"));
            if (!configFile.Exists)
            {
                stringBuilder.AppendLine(configFile.FullName);
                configFile =
                    new FileInfo(Path.Combine(baseDirectory, $"{fileName}"));
            }
            if (!configFile.Exists)
            {
                stringBuilder.AppendLine(configFile.FullName);
                configFile =
                    new FileInfo(Path.Combine(baseDirectory, "bin",
                        $"{fileName}"));
            }
            if (!configFile.Exists)
            {
                serilogEventLog.WriteEntry($"Can not find the configuration in following path {stringBuilder}", EventLogEntryType.Warning);
                return string.Empty;
            }
            else
            {
                return configFile.FullName;
            }
        }

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
                for (int i = 0; i < 30; i++)
                {
                    Log.Information($"{guid}, {i}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                    Thread.Sleep(500);
                }

                Log.Information($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.Error($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.Debug($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
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

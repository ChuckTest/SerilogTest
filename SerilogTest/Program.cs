using System;
using System.IO;
using System.Reflection;
using Serilog;

namespace SerilogTest
{
    internal class Program
    {
        static string GetConfigPath()
        {
            const string testsConfig = "serilog.config";
            if (File.Exists(testsConfig))
                return Path.GetFullPath(testsConfig);
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(Path.Combine(basePath, testsConfig));
        }

        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine(assembly.FullName);
            try
            {
                var path = GetConfigPath();
                Guid guid = Guid.NewGuid();

                var loggerConfiguration = new LoggerConfiguration();
                loggerConfiguration = loggerConfiguration.ReadFrom.AppSettings(filePath: path);
                var logger = loggerConfiguration.CreateLogger();
                Log.Logger = logger;
                //for (int i = 0; i < 1000; i++)
                //{
                //    Log.Information($"{guid}, {i}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                //}
                Log.Information($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.Error($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}

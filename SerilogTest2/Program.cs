using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Serilog;
using Serilog.Debugging;

namespace SerilogTest2
{
    class Program
    {
        static string GetConfigPath()
        {
            const string testsConfig = "serilog.config";
            if (File.Exists(testsConfig))
                return Path.GetFullPath(testsConfig);
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(Path.Combine(basePath, testsConfig));
        }
        internal static IEnumerable<KeyValuePair<string, Assembly>> GetAutoLoadingFileLocations()
        {
            var nlogAssembly = typeof(ILogger).GetAssembly();
            var assemblyLocation = PathHelpers.TrimDirectorySeparators(AssemblyHelpers.GetAssemblyFileLocation(nlogAssembly));
            if (!string.IsNullOrEmpty(assemblyLocation))
                yield return new KeyValuePair<string, Assembly>(assemblyLocation, nlogAssembly);

            var entryAssembly = Assembly.GetEntryAssembly();
            var entryLocation = PathHelpers.TrimDirectorySeparators(AssemblyHelpers.GetAssemblyFileLocation(Assembly.GetEntryAssembly()));
            if (!string.IsNullOrEmpty(entryLocation) && !string.Equals(entryLocation, assemblyLocation, StringComparison.OrdinalIgnoreCase))
                yield return new KeyValuePair<string, Assembly>(entryLocation, entryAssembly);

            // TODO Consider to prioritize AppDomain.PrivateBinPath
            var baseDirectory = PathHelpers.TrimDirectorySeparators(AppDomain.CurrentDomain.BaseDirectory);
            //InternalLogger.Debug("Auto loading based on AppDomain-BaseDirectory found location: {0}", baseDirectory);
            if (!string.IsNullOrEmpty(baseDirectory) && !string.Equals(baseDirectory, assemblyLocation, StringComparison.OrdinalIgnoreCase))
                yield return new KeyValuePair<string, Assembly>(baseDirectory, null);
        }

        static void Main(string[] args)
        {
            try
            {
                AutoLoad();

                SelfLog.Enable(SelfLogHandler);

                var path = GetConfigPath();
                Guid guid = Guid.NewGuid();

                var loggerConfiguration = new LoggerConfiguration();
                loggerConfiguration = loggerConfiguration.ReadFrom.AppSettings(filePath: path);
                var logger = loggerConfiguration.CreateLogger();
                Log.Logger = logger;
                for (int i = 0; i < 30; i++)
                {
                    Log.Information($"{guid}, {i}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                    Thread.Sleep(500);
                }
                Log.Information($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.Error($"{guid}, this is a test log {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz}.");
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        static void AutoLoad()
        {
            var nlogAssembly = typeof(ILogger).GetAssembly();
            var assemblyLocation = string.Empty;
            var extensionDlls = ArrayHelper.Empty<string>();
            var fileLocations = GetAutoLoadingFileLocations();
            foreach (var fileLocation in fileLocations)
            {
                if (string.IsNullOrEmpty(fileLocation.Key))
                    continue;

                if (string.IsNullOrEmpty(assemblyLocation))
                    assemblyLocation = fileLocation.Key;

                extensionDlls = GetNLogExtensionFiles(fileLocation.Key);
                if (extensionDlls.Length > 0)
                {
                    assemblyLocation = fileLocation.Key;
                    break;
                }
            }
            Console.WriteLine($"{nameof(nlogAssembly)} = {nlogAssembly}");
            Console.WriteLine($"{nameof(extensionDlls)}:");
            int i = 0;
            foreach (var item in extensionDlls)
            {
                i++;
                Console.WriteLine($"{i}, {item}");
            }
            Console.WriteLine("Start auto loading, location: {0}", assemblyLocation);
            Console.WriteLine("LoadNLogExtensionAssemblies(factory, nlogAssembly, extensionDlls);");
        }

        private static string[] GetNLogExtensionFiles(string assemblyLocation)
        {
            try
            {
                //InternalLogger.Debug("Search for auto loading files in location: {0}", assemblyLocation);
                if (string.IsNullOrEmpty(assemblyLocation))
                {
                    return ArrayHelper.Empty<string>();
                }

                const string libraryName = "Serilog";
                var extensionDlls = Directory.GetFiles(assemblyLocation, $"{libraryName}*.dll")
                .Select(Path.GetFileName)
                .Where(x => !x.Equals($"{libraryName}.dll", StringComparison.OrdinalIgnoreCase))
                .Where(x => !x.Equals($"{libraryName}.UnitTests.dll", StringComparison.OrdinalIgnoreCase))
                .Select(x => Path.Combine(assemblyLocation, x));
                return extensionDlls.ToArray();
            }
            catch (DirectoryNotFoundException ex)
            {
                //InternalLogger.Warn(ex, "Skipping auto loading location because assembly directory does not exist: {0}", assemblyLocation);
                //if (ex.MustBeRethrown())
                {
                    throw;
                }
                return ArrayHelper.Empty<string>();
            }
            catch (System.Security.SecurityException ex)
            {
                //InternalLogger.Warn(ex, "Skipping auto loading location because access not allowed to assembly directory: {0}", assemblyLocation);
                //if (ex.MustBeRethrown())
                {
                    throw;
                }
                return ArrayHelper.Empty<string>();
            }
            catch (UnauthorizedAccessException ex)
            {
                //InternalLogger.Warn(ex, "Skipping auto loading location because access not allowed to assembly directory: {0}", assemblyLocation);
                //if (ex.MustBeRethrown())
                {
                    throw;
                }
                return ArrayHelper.Empty<string>();
            }
        }

        static readonly SerilogEventLog serilogEventLog = new SerilogEventLog();
        static void SelfLogHandler(string log)
        {
            serilogEventLog.WriteEntry(log, EventLogEntryType.Error);
        }
    }
}

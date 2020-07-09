using System;
using Serilog;

namespace SerilogTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Guid guid = Guid.NewGuid();

                Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings()
                    .CreateLogger();

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
    }
}

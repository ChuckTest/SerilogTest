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

                Log.Information($"{guid}, Hello, world!");

                int a = 10, b = 0;
                try
                {
                    Log.Debug($"{guid}, Dividing {a} by {b}");
                    Console.WriteLine(a / b);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"{guid}, Something went wrong");
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

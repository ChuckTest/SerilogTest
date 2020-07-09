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
                Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings()
                    .CreateLogger();

                Log.Information("Hello, world!");

                int a = 10, b = 0;
                try
                {
                    Log.Debug("Dividing {A} by {B}", a, b);
                    Console.WriteLine(a / b);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Something went wrong");
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

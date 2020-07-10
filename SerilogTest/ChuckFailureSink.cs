using System;
using Serilog.Core;
using Serilog.Events;

namespace SerilogTest
{
    class ChuckFailureSink : ILogEventSink
    {
        void ILogEventSink.Emit(LogEvent logEvent)
        {
            //throw new NotSupportedException();
            var e = logEvent;
            Console.WriteLine($"Unable to submit event {e.MessageTemplate},{Environment.NewLine}Exception: {e.Exception}");
        }
    }
}

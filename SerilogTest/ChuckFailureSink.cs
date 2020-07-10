using System;
using System.Diagnostics;
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
            if (e.Exception != null)
            {
                var content = $"{e.Exception}";
                SerilogEventLog lisaEventLog = new SerilogEventLog();
                lisaEventLog.WriteEntry(content, EventLogEntryType.Error);
            }
        }
    }
}

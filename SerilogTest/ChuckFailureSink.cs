using System;
using Serilog.Core;
using Serilog.Events;

namespace SerilogTest
{
    class ChuckFailureSink : ILogEventSink
    {
        void ILogEventSink.Emit(LogEvent logEvent)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Diagnostics;

namespace SerilogTest
{
    internal class SerilogEventLog
    {
        private readonly string _logName = @"Serilog";

        public string LogName => _logName;

        public SerilogEventLog()
        {
        }

        public SerilogEventLog(string logName)
        {
            _logName = logName;
        }

        public void WriteEntry(string error, EventLogEntryType type)
        {
            var sourceName = AppDomain.CurrentDomain.FriendlyName;
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, _logName);
            }

            using (EventLog eventLog = new EventLog(_logName))
            {
                eventLog.Source = sourceName;
                var message = $"{AppDomain.CurrentDomain.BaseDirectory}{Environment.NewLine}{error}";
                eventLog.WriteEntry(message, type);
            }
        }
    }
}

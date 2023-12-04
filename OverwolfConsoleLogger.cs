using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overwolf.plugins
{
    public class LogMessage
    {
        public string level { get; set; }
        public string message { get; set; }
    }

    public delegate void OnLogLineEvent(LogMessage message);

    public class OverwolfConsoleLogger : ILogger
    {
        public event OnLogLineEvent OnLogLine;

        public LogLevel Level { get; set; } = LogLevel.Info;

        public OverwolfConsoleLogger()
        {
            this.Level = LogLevel.Info;
        }

        public OverwolfConsoleLogger(LogLevel level)
        {
            Level = level;
        }

        public void Error(string message, params object[] args)
        {
            if (Level > LogLevel.Error) return;
            OnLogLine?.Invoke(new LogMessage() { level = "ERROR", message = (args.Length > 0 ? string.Format(message, args) : message) });
        }

        public void Info(string message, params object[] args)
        {
            if (Level > LogLevel.Info) return;
            OnLogLine?.Invoke(new LogMessage() { level = "INFO", message = (args.Length > 0 ? string.Format(message, args) : message) });
        }

        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;
            OnLogLine?.Invoke(new LogMessage() { level = "TRACE", message = (args.Length > 0 ? string.Format(message, args) : message) });
        }

        public void Warning(string message, params object[] args)
        {
            if (Level > LogLevel.Warning) return;
            OnLogLine?.Invoke(new LogMessage() { level = "WARNING", message = (args.Length > 0 ? string.Format(message, args) : message) });
        }
    }
}

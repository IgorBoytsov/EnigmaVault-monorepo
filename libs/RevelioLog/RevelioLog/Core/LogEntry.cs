using Microsoft.Extensions.Logging;
using RevelioLog.Core.Enums;

namespace RevelioLog.Core
{
    public class LogEntry
    {
        public DateTime Timestamp { get; }
        public Level Level { get; }
        public string Message { get; }
        public Exception? Exception { get; }
        public string? CallerMemberName { get; }

        public LogEntry(Level level, string message, Exception? exception = null, string? callerMemberName = null)
        {
            Timestamp = DateTime.UtcNow;
            Level = level;
            Message = message;
            Exception = exception;
            CallerMemberName = callerMemberName;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append($"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level.ToString().ToUpper()}]");

            if (!string.IsNullOrEmpty(CallerMemberName))
                sb.Append($" ({CallerMemberName})");

            sb.Append($" {Message}");

            if (Exception != null)
                sb.Append($"{Environment.NewLine}{Exception}");

            return sb.ToString();
        }
    }
}
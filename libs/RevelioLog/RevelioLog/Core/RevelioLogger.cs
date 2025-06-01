using Microsoft.Extensions.Options;
using RevelioLog.Configurations;
using RevelioLog.Core.Abstractions;
using RevelioLog.Core.Enums;
using RevelioLog.Targets.Abstractions;
using System.Runtime.CompilerServices;

namespace RevelioLog.Core
{
    public class RevelioLogger : IRevelioLog
    {
        private readonly IEnumerable<IRevelioLogTarget> _targets;
        public Level GlobalMinimumLevel { get; }

        public RevelioLogger(IEnumerable<IRevelioLogTarget> targets, IOptions<LoggerOptions> options)
        {
            _targets = targets?.ToList() ?? new List<IRevelioLogTarget>();
            GlobalMinimumLevel = options?.Value?.GlobalMinimumLevel ?? Level.Trace;
        }

        public void Log(Level level, string message, Exception? ex = null, [CallerMemberName] string callerMemberName = "")
        {
            if (level < GlobalMinimumLevel)
                return;

            var logEntry = new LogEntry(level, message, ex, callerMemberName);

            foreach (var target in _targets)
            {
                try
                {
                    target.Write(logEntry);
                }
                catch (Exception targetEx)
                {
                    Console.Error.WriteLine($"Error in target {target.Name}: {targetEx.Message}");
                }
            }
        }

        public void Trace(string message, [CallerMemberName] string c = "") => Log(Level.Trace, message, null, c);

        public void Debug(string message, [CallerMemberName] string c = "") => Log(Level.Debug, message, null, c);

        public void Information(string message, [CallerMemberName] string c = "") => Log(Level.Information, message, null, c);

        public void Warning(string message, Exception? ex = null, [CallerMemberName] string c = "") => Log(Level.Warning, message, ex, c);

        public void Error(string message, Exception? ex = null, [CallerMemberName] string c = "") => Log(Level.Error, message, ex, c);

        public void Fatal(string message, Exception? ex = null, [CallerMemberName] string c = "") => Log(Level.Fatal, message, ex, c);

    }
}
using RevelioLog.Core;
using RevelioLog.Core.Enums;
using RevelioLog.Targets.Abstractions;

namespace RevelioLog.Targets
{
    public class ConsoleTarget : IRevelioLogTarget
    {
        public string Name { get; } = "ConsoleTarget";
        public Level MinimumLevel { get; set; } = Level.Trace;

        public void Write(LogEntry logEntry)
        {
            if (logEntry.Level >= MinimumLevel)
                Console.WriteLine(logEntry.ToString());
        }
    }
}

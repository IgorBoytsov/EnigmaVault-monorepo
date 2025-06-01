using RevelioLog.Core;
using RevelioLog.Core.Enums;
using RevelioLog.Targets.Abstractions;
using System.Diagnostics;

namespace RevelioLog.Targets
{
    public class DebugTarget : IRevelioLogTarget
    {
        public string Name { get; } = "DebugTarget";
        public Level MinimumLevel { get; set; } = Level.Trace;

        public void Write(LogEntry logEntry)
        {
            if (logEntry.Level >= MinimumLevel)
                Debug.WriteLine(logEntry.ToString());
        }
    }
}
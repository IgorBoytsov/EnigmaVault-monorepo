using RevelioLog.Core;
using RevelioLog.Core.Enums;

namespace RevelioLog.Targets.Abstractions
{
    public interface IRevelioLogTarget
    {
        string Name { get; }
        Level MinimumLevel { get; set; }
        void Write(LogEntry logEntry);
    }
}
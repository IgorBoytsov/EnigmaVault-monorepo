using RevelioLog.Core.Enums;
using System.Runtime.CompilerServices;

namespace RevelioLog.Core.Abstractions
{
    public interface IRevelioLog
    {
        Level GlobalMinimumLevel { get; }

        void Fatal(string message, Exception? ex = null, [CallerMemberName] string c = "");
        void Debug(string message, [CallerMemberName] string c = "");
        void Error(string message, Exception? ex = null, [CallerMemberName] string c = "");
        void Information(string message, [CallerMemberName] string c = "");
        void Log(Level level, string message, Exception? ex = null, [CallerMemberName] string callerMemberName = "");
        void Trace(string message, [CallerMemberName] string c = "");
        void Warning(string message, Exception? ex = null, [CallerMemberName] string c = "");
    }
}
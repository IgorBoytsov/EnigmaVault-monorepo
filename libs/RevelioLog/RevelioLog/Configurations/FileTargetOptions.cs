using RevelioLog.Core.Enums;

namespace RevelioLog.Configurations
{
    public class FileTargetOptions
    {
        public Level MinimumLevel { get; set; } = Level.Information;
        public string FilePath { get; set; } = "default.log";
    }
}
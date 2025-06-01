using Microsoft.Extensions.Options;
using RevelioLog.Configurations;

namespace RevelioLog.Targets
{
    internal class FileTargetWrapper : FileTarget 
    {
        public FileTargetWrapper(IOptions<FileTargetOptions> options) : base(options?.Value?.FilePath ?? "default_di.log") 
        {
            if (options?.Value != null)
                this.MinimumLevel = options.Value.MinimumLevel;
        }
    }
}
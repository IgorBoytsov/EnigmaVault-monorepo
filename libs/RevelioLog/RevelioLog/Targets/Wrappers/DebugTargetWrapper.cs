using Microsoft.Extensions.Options;
using RevelioLog.Configurations;

namespace RevelioLog.Targets.Wrappers
{
    internal class DebugTargetWrapper : DebugTarget
    {
        public DebugTargetWrapper(IOptions<DebugTargetOptions> options) : base()
        {
            if (options?.Value != null)
                this.MinimumLevel = options.Value.MinimumLevel;
        }
    }
}
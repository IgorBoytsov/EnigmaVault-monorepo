using Microsoft.Extensions.Options;
using RevelioLog.Configurations;

namespace RevelioLog.Targets
{
    internal class ConsoleTargetWrapper : ConsoleTarget
    {
        public ConsoleTargetWrapper(IOptions<ConsoleTargetOptions> options) : base()
        {
            if (options?.Value != null)
                this.MinimumLevel = options.Value.MinimumLevel;
        }
    }
}
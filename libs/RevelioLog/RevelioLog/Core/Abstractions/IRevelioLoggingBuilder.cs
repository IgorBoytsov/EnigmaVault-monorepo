using Microsoft.Extensions.DependencyInjection;

namespace RevelioLog.Core.Abstractions
{
    public interface IRevelioLoggingBuilder
    {
        IServiceCollection Services { get; }
    }
}

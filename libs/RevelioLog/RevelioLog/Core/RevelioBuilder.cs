using Microsoft.Extensions.DependencyInjection;
using RevelioLog.Core.Abstractions;

namespace RevelioLog.Core
{
    public class RevelioBuilder : IRevelioLoggingBuilder
    {
        public IServiceCollection Services { get; }
        public RevelioBuilder(IServiceCollection services) => Services = services;
    }
}
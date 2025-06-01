using Microsoft.Extensions.DependencyInjection;
using RevelioLog.Configurations;
using RevelioLog.Core;
using RevelioLog.Core.Abstractions;

namespace RevelioLog.Extensions
{
    public static class RevelioLoggerServiceCollectionExtensions
    {
        public static IRevelioLoggingBuilder AddRevelioLogger(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IRevelioLog, RevelioLogger>();

            services.AddOptions<LoggerOptions>();

            return new RevelioBuilder(services);
        }

        public static IRevelioLoggingBuilder AddRevelioLogger(this IServiceCollection services, Action<LoggerOptions> configureOptions)
        {
            var builder = services.AddRevelioLogger();
            services.Configure(configureOptions);
            return builder;
        }
    }
}
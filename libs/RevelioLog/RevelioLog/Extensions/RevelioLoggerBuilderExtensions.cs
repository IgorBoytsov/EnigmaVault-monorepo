using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RevelioLog.Configurations;
using RevelioLog.Core.Abstractions;
using RevelioLog.Targets;
using RevelioLog.Targets.Abstractions;

namespace RevelioLog.Extensions
{
    public static class RevelioLoggerBuilderExtensions
    {
        /*--Debug-----------------------------------------------------------------------------------------*/

        public static IRevelioLoggingBuilder AddDebug(this IRevelioLoggingBuilder builder)
        {
            builder.Services.AddOptions<DebugTargetOptions>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRevelioLogTarget, DebugTarget>());

            return builder;
        }

        public static IRevelioLoggingBuilder AddDebug(this IRevelioLoggingBuilder builder, Action<DebugTargetOptions> configureOptions)
        {
            builder.AddDebug();
            builder.Services.Configure(configureOptions);

            return builder;
        }

        /*--Console---------------------------------------------------------------------------------------*/

        public static IRevelioLoggingBuilder AddConsole(this IRevelioLoggingBuilder builder)
        {
            builder.Services.AddOptions<ConsoleTargetOptions>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRevelioLogTarget, ConsoleTargetWrapper>());
            return builder;
        }

        public static IRevelioLoggingBuilder AddConsole(this IRevelioLoggingBuilder builder, Action<ConsoleTargetOptions> configureOptions)
        {
            builder.AddConsole(); 
            builder.Services.Configure(configureOptions);
            return builder;
        }

        /*--File------------------------------------------------------------------------------------------*/
      
        public static IRevelioLoggingBuilder AddFile(this IRevelioLoggingBuilder builder)
        {
            builder.Services.AddOptions<FileTargetOptions>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IRevelioLogTarget, FileTargetWrapper>());
            return builder;
        }

        public static IRevelioLoggingBuilder AddFile(this IRevelioLoggingBuilder builder, Action<FileTargetOptions> configureOptions)
        {
            builder.AddFile();
            builder.Services.Configure(configureOptions);
            return builder;
        }
    }
}
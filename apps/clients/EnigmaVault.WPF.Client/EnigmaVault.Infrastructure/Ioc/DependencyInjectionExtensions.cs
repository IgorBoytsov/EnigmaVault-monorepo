using EnigmaVault.Infrastructure.Services.Abstractions;
using EnigmaVault.Infrastructure.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace EnigmaVault.Infrastructure.Ioc
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IApiRequestHandler, HttpClientRequestHandler>();

            return services;
        }
    }
}
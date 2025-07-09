using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Infrastructure.Repositories;
using EnigmaVault.SecretService.Infrastructure.Services.Abstractions;
using EnigmaVault.SecretService.Infrastructure.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace EnigmaVault.SecretService.Infrastructure.Ioc
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ISecretRepository, SecretRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IEntityUpdater, EntityUpdater>(); 

            return services;
        }
    }
}
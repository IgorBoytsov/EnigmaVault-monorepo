using EnigmaVault.SecretService.Application.Abstractions;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Infrastructure;
using EnigmaVault.SecretService.Infrastructure.Data;
using EnigmaVault.SecretService.Infrastructure.Ioc;
using Microsoft.EntityFrameworkCore;

namespace EnigmaVault.SecretService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<SecretDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ISecretRepository).Assembly));

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddInfrastructureServices();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
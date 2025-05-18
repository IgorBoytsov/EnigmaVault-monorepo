
using EnigmaVault.AuthenticationService.Api.Exceptions;
using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Providers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using EnigmaVault.AuthenticationService.Application.Implementations.Providers;
using EnigmaVault.AuthenticationService.Application.Implementations.UseCases;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using EnigmaVault.AuthenticationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EnigmaVault.AuthenticationService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            #region Дополнительные зависимости

             var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidConnectionStringException("Строка подключения 'DefaultConnection' не найдена.");

            builder.Services.AddDbContext<UsersDBContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddSingleton<IRegistrationErrorMessageProvider, DefaultRegistrationErrorMessageProvider>();
            builder.Services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
                app.MapOpenApi();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
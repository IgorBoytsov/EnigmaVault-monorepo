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
using Serilog;

namespace EnigmaVault.AuthenticationService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() 
                .CreateBootstrapLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Services.AddControllers();
                builder.Services.AddOpenApi();

                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrEmpty(connectionString))
                    throw new InvalidConnectionStringException("Строка подключения 'DefaultConnection' не найдена.");

                AddDbContext(builder.Services, connectionString);
                AddLogger(builder);
                AddRepositories(builder.Services);
                AddUseCases(builder.Services);
                AddProviders(builder.Services);

                builder.Services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();

                var app = builder.Build();

                if (app.Environment.IsDevelopment())
                    app.MapOpenApi();

                app.UseSerilogRequestLogging();

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Приложение не запустилось");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void AddDbContext(IServiceCollection services, string connectionString) => services.AddDbContext<UsersDBContext>(options => options.UseSqlServer(connectionString));

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
            services.AddScoped<IAuthenticateUserUseCase, AuthenticateUserUseCase>();
            services.AddScoped<IRecoveryAccessUserUseCase, RecoveryAccessUserUseCase>();
            services.AddScoped<IGetUserByLoginUseCase, GetUserByLoginUseCase>();

            services.AddScoped<IGetAllCountryStreamingUseCase, GetAllCountryStreamingUseCase>();
            services.AddScoped<IGetAllGenderStreamingUseCase, GetAllGenderStreamingUseCase>();
        } 
        
        private static void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IGenderRepository, GenderRepository>();
        } 
        
        private static void AddProviders(IServiceCollection services)
        {
            services.AddSingleton<IDefaultErrorMessageProvider, DefaultRegistrationErrorMessageProvider>();
            services.AddSingleton<IDefaultErrorMessageProvider, DefaultAuthenticateErrorMessageProvider>();
        }

        private static void AddLogger(WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((hostingContext, loggerConfig) =>
            {
                loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext();
            });
        }
    }
}
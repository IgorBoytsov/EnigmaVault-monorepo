using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.AutoMappers.Profiles;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.Services.Implementations;
using EnigmaVault.Application.UseCases.Abstractions.CountryCase;
using EnigmaVault.Application.UseCases.Abstractions.GanderCase;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Application.UseCases.Implementations.CountryCase;
using EnigmaVault.Application.UseCases.Implementations.GenderCase;
using EnigmaVault.Application.UseCases.Implementations.UserCase;
using EnigmaVault.Infrastructure.Repositories;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Factories.Abstractions;
using EnigmaVault.WPF.Client.Factories.Pages;
using EnigmaVault.WPF.Client.Factories.Windows;
using EnigmaVault.WPF.Client.Services.Abstractions;
using EnigmaVault.WPF.Client.Services.Implementations;
using EnigmaVault.WPF.Client.ViewModels.Pages;
using EnigmaVault.WPF.Client.ViewModels.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace EnigmaVault.WPF.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public IServiceProvider ServiceProvider { get; private set; } = null!;
        public static IHost AppHost { get; private set; } = null!;

        public static string? BaseApiUrl { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("apiconfig.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = configurationBuilder.Build();

            BaseApiUrl = configuration.GetValue<string>("BaseAuthenticationServiceUrl");

            //TODO: Сделать кастомное исключение
            if (string.IsNullOrEmpty(BaseApiUrl))
                throw new Exception("Ключ 'BaseAuthenticationServiceUrl' не найден или пуст в файле apiconfig.json!");

            ConfigureServices(services);
            ConfigureAutoMapper(services);
            ConfigureWindow(services);
            ConfigurePage(services);
            ConfigureUseCases(services);
            ConfigureHttpAndRepositories(services);

            ServiceProvider = services.BuildServiceProvider();

            ServiceProvider.GetService<IWindowNavigationService>()!.Open(WindowName.AuthenticationWindow);

            base.OnStartup(e);
        }

        private static void ConfigureWindow(ServiceCollection services)
        {
            services.AddTransient<IWindowFactory, AuthenticationWindowFactory>();
            services.AddTransient<AuthenticationWindowVM>();
            services.AddSingleton<Func<AuthenticationWindowVM>>(provider => () => provider.GetRequiredService<AuthenticationWindowVM>());

            services.AddTransient<IWindowFactory, MainWindowFactory>();
            services.AddTransient<MainWindowVM>();
            services.AddSingleton<Func<MainWindowVM>>(provider => () => provider.GetRequiredService<MainWindowVM>());
        }

        private static void ConfigurePage(ServiceCollection services)
        {
            services.AddTransient<IPageFactory, ProfilePageFactory>();
            services.AddTransient<ProfilePageVM>();
            services.AddSingleton<Func<ProfilePageVM>>(provider => () => provider.GetRequiredService<ProfilePageVM>());
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IWindowNavigationService, WindowNavigationService>();
            services.AddSingleton<IPageNavigationService, PageNavigationService>();

            services.AddSingleton<IAuthorizationService, AuthorizationService>();
        }
       
        private static void ConfigureUseCases(ServiceCollection services)
        {
            services.AddScoped<IAuthenticationUserUseCase, AuthenticationUserUseCase>();
            services.AddScoped<IRegistrationUserUseCase, RegistrationUserUseCase>();
            services.AddScoped<IRecoveryAccessUserUseCase, RecoveryAccessUserUseCase>();

            services.AddScoped<IGetGendersUseCase, GetGendersUseCase>();
            services.AddScoped<IGetCountriesUseCase, GetCountriesUseCase>();
        }

        private static void ConfigureHttpAndRepositories(ServiceCollection services)
        {
            var authApi = "AuthApiClient";
            services.AddHttpClient(authApi, client => client.BaseAddress = new Uri(BaseApiUrl!));

            services.AddHttpClient<IUserRepository, ApiUserRepository>(authApi);
            services.AddHttpClient<IGenderRepository, ApiGenderRepository>(authApi);
            services.AddHttpClient<ICountryRepository, ApiCountryRepository>(authApi);
        }

        private static void ConfigureAutoMapper(ServiceCollection services) => services.AddAutoMapper(typeof(UserProfile).Assembly);

    }
}
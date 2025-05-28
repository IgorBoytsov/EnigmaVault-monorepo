using EnigmaVault.WPF.Client.Factories.Abstractions;
using EnigmaVault.WPF.Client.ViewModels.Windows;
using EnigmaVault.WPF.Client.Views.Windows;
using System.Windows;

namespace EnigmaVault.WPF.Client.Factories.Windows
{
    internal class AuthenticationWindowFactory(Func<AuthenticationWindowVM> viewModelFactory) : IWindowFactory
    {
        private readonly Func<AuthenticationWindowVM> _viewModelFactory = viewModelFactory;

        public Window CreateWindow()
        {
            var viewModel = _viewModelFactory();
            var window = new AuthenticationWindow { DataContext = viewModel };

            return window;
        }
    }
}
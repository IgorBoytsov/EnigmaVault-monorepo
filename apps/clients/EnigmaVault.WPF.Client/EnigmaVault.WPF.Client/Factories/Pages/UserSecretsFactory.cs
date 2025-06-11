using EnigmaVault.WPF.Client.Factories.Abstractions;
using EnigmaVault.WPF.Client.ViewModels.Pages;
using EnigmaVault.WPF.Client.Views.Pages;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Factories.Pages
{
    internal class UserSecretsFactory(Func<UserSecretsPageVM> viewModelFactory) : IPageFactory
    {
        private readonly Func<UserSecretsPageVM> _viewModelFactory = viewModelFactory;

        public Page CreatePage()
        {
            var viewModel = _viewModelFactory();
            var page = new UserSecretsPage { DataContext = viewModel };

            return page;
        }
    }
}
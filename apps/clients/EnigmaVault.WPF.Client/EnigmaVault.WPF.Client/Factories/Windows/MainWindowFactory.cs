using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Factories.Abstractions;
using EnigmaVault.WPF.Client.Services.Abstractions;
using EnigmaVault.WPF.Client.ViewModels.Windows;
using EnigmaVault.WPF.Client.Views.Windows;
using System.Windows;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Factories.Windows
{
    internal class MainWindowFactory(Func<MainWindowVM> viewModelFactory, IPageNavigationService pageNavigationService) : IWindowFactory
    {
        private readonly Func<MainWindowVM> _viewModelFactory = viewModelFactory;
        
        private readonly IPageNavigationService _pageNavigationService = pageNavigationService;

        public Window CreateWindow()
        {
            var viewModel = _viewModelFactory();
            var window = new MainWindow { DataContext = viewModel };

            window.Loaded += (sender, args) =>
            {
                var mainFrame = window.FindName("MainFrame") as Frame;
                _pageNavigationService.RegisterFrame(FrameName.MainFrame, mainFrame);
                _pageNavigationService.Navigate(PageName.ProfilePage, FrameName.MainFrame);
            };

            return window;
        }
    }
}
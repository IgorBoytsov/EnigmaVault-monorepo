using EnigmaVault.WPF.Client.Command;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Services.Abstractions;

namespace EnigmaVault.WPF.Client.ViewModels.Windows
{
    public abstract class BaseWindowViewModel : BaseViewModel
    {
        private readonly IWindowNavigationService? _windowNavigationService;

        internal BaseWindowViewModel() { }

        internal BaseWindowViewModel(IWindowNavigationService windowNavigationService)
        {
            _windowNavigationService = windowNavigationService;
            SetValueCommands();
        }

        private string _windowTitle = null!;
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }

        /*--Команды---------------------------------------------------------------------------------------*/

        private void SetValueCommands()
        {
            ShutDownAppCommand = new RelayCommand(Execute_ShutDownAppCommand);

            MinimizeWindowCommand = new RelayCommand<WindowName>(Execute_MinimizeWindowCommand);
            MaximizeWindowCommand = new RelayCommand<WindowName>(Execute_MaximizeWindowCommand);
            RestoreWindowCommand = new RelayCommand<WindowName>(Execute_RestoreWindowCommand);
            CloseWindowCommand = new RelayCommand<WindowName>(Execute_CloseWindowCommand);
        }

        #region ShutDownApp

        public RelayCommand? ShutDownAppCommand { get; private set; }

        private void Execute_ShutDownAppCommand() => System.Windows.Application.Current.Shutdown();

        #endregion

        #region Minimize

        public RelayCommand<WindowName>? MinimizeWindowCommand { get; private set; }

        private void Execute_MinimizeWindowCommand(WindowName windowName) => _windowNavigationService!.MinimizeWindow(windowName);

        #endregion

        #region Maximize

        public RelayCommand<WindowName>? MaximizeWindowCommand { get; private set; }

        private void Execute_MaximizeWindowCommand(WindowName windowName) => _windowNavigationService!.MaximizeWindow(windowName);

        #endregion

        #region Restore

        public RelayCommand<WindowName>? RestoreWindowCommand { get; private set; }

        private void Execute_RestoreWindowCommand(WindowName windowName) => _windowNavigationService!.RestoreWindow(windowName);

        #endregion

        #region Close

        public RelayCommand<WindowName>? CloseWindowCommand { get; private set; }

        private void Execute_CloseWindowCommand(WindowName windowName) => _windowNavigationService!.Close(windowName);

        #endregion

    }
}
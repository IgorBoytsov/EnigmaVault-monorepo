using EnigmaVault.WPF.Client.Command;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Services.Abstractions;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;

namespace EnigmaVault.WPF.Client.ViewModels.Windows
{
    internal class MainWindowVM : BaseWindowViewModel, IUpdatable
    {
        private readonly IPageNavigationService _pageNavigationService;

        public MainWindowVM(IPageNavigationService pageNavigationService,
                            IWindowNavigationService windowNavigationService) : base(windowNavigationService)
        {
            _pageNavigationService = pageNavigationService;
            SetValueCommands();
            CurrentOpenPage = PageName.UserSecrets; // Текущая открытая страница. Открывается изначально в MainWindowFactory
        }

        void IUpdatable.Update<TData>(TData value, TransmittingParameter parameter)
        {

        }

        /*--Коллекции-------------------------------------------------------------------------------------*/


        /*--Свойства--------------------------------------------------------------------------------------*/

        private PageName _currentOpenPage;
        public PageName CurrentOpenPage
        {
            get => _currentOpenPage;
            set
            {
                SetProperty(ref _currentOpenPage, value);
                OpenProfilePageCommand?.RaiseCanExecuteChanged();
                OpenSecretPageCommand?.RaiseCanExecuteChanged();
            }
        }

        /*--Команды---------------------------------------------------------------------------------------*/

        private void SetValueCommands()
        {
            OpenProfilePageCommand = new RelayCommand(Execute_OpenProfilePageCommand, CanExecute_OpenProfilePageCommand);
            OpenSecretPageCommand = new RelayCommand(Execute_OpenSecretPageCommand, CanExecute_OpenSecretPageCommand);
        }

        #region Открыть страницу с профилем

        public RelayCommand? OpenProfilePageCommand { get; private set; }

        private void Execute_OpenProfilePageCommand()
        {
            _pageNavigationService.Navigate(PageName.ProfilePage, FrameName.MainFrame);
            CurrentOpenPage = _pageNavigationService.GetCurrentDisplayedPage(FrameName.MainFrame);
        }

        private bool CanExecute_OpenProfilePageCommand() => CurrentOpenPage != PageName.ProfilePage;

        #endregion 
        
        #region Открыть страницу с секретами

        public RelayCommand? OpenSecretPageCommand { get; private set; }

        private void Execute_OpenSecretPageCommand()
        {
            _pageNavigationService.Navigate(PageName.UserSecrets, FrameName.MainFrame);
            CurrentOpenPage = _pageNavigationService.GetCurrentDisplayedPage(FrameName.MainFrame);
        }

        private bool CanExecute_OpenSecretPageCommand() => CurrentOpenPage != PageName.UserSecrets;

        #endregion

        /*--Методы----------------------------------------------------------------------------------------*/
    }
}
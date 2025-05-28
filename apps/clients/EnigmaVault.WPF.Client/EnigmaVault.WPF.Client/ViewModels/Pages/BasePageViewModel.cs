namespace EnigmaVault.WPF.Client.ViewModels.Pages
{
    public abstract class BasePageViewModel : BaseViewModel
    {
        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        // Вызывается, когда страница становится активной
        public virtual void OnNavigatedTo(object parameter)
        {
            // Логика инициализации при переходе на эту страницу/представление
            // parameter - данные, переданные при навигации
        }

        // Вызывается, когда уходят с этой страницы
        public virtual void OnNavigatedFrom()
        {
            // Логика очистки при уходе с этой страницы/представления
        }

        // Можно добавить метод для асинхронной загрузки данных, если нужно
        public virtual Task LoadDataAsync()
        {
            // Загрузка данных для страницы
            return Task.CompletedTask;
        }
    }
}
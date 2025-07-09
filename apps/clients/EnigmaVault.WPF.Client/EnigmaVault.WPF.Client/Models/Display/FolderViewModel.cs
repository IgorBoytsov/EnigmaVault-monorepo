using EnigmaVault.Application.Dtos.Secrets.Folders;

namespace EnigmaVault.WPF.Client.Models.Display
{
    internal class FolderViewModel : ObservableObject
    {
        private readonly FolderDto _model;

        public const string DISPLAYE_SECRETS_SYSTEM_FOLDER_NAME = "Не назначена";
        public const string SYSTEM_FOLDER_NAME = "Все секреты";

        public FolderViewModel(FolderDto model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public int IdFolder => _model.IdFolder;
        public int IdUser => _model.IdUser;

        public string FolderName
        {
            get => _model.FolderName;
            private set
            {
                if (_model.FolderName != value)
                {
                    _model.FolderName = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _countElement;
        public int CountElement
        {
            get => _countElement;
            private set
            {
                SetProperty(ref _countElement, value);
            }
        }

        public void SetCountElements(int count)
        {
            if (count < 0)
                throw new Exception("Элементов не может быть меньшее 0");

            CountElement = count;
        }

        public void SetName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new Exception("Название пустое, пожалуйста укажите корректное название");

            FolderName = newName;
        }

        public FolderDto GetUnderlyingModel() => _model;
    }
}
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using System.Windows.Media;

namespace EnigmaVault.WPF.Client.Models.Display
{
    internal sealed class EncryptedSecretViewModel : ObservableObject
    {
        private readonly EncryptedSecret _model;

        public EncryptedSecretViewModel(EncryptedSecret model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public int IdSecret => _model.IdSecret;
        public int SchemaVersion => _model.SchemaVersion;
        public string? SvgCode => _model.SvgIcon;

        public int? IdFolder
        {
            get => _model.IdFolder;
            private set
            {
                if (_model.IdFolder != value)
                {
                    _model.IdFolder = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _folderName;
        public string? FolderName
        {
            get => _folderName;
            private set
            {
                SetProperty(ref _folderName, value);
            }
        }

        public string EncryptedData
        {
            get => _model.EncryptedData;
            set
            {
                if (_model.EncryptedData != value)
                {
                    _model.EncryptedData = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Nonce
        {
            get => _model.Nonce;
            set
            {
                if (_model.Nonce != value)
                {
                    _model.Nonce = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ServiceName
        {
            get => _model.ServiceName;
            set
            {
                if (_model.ServiceName != value)
                {
                    _model.ServiceName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ServiceNameFirstLetter));
                }
            }
        }

        public string? Url
        {
            get => _model.Url;
            set
            {
                if (_model.Url != value)
                {
                    _model.Url = value!;
                    OnPropertyChanged();
                }
            }
        }

        public string? Notes
        {
            get => _model.Notes;
            set
            {
                if (_model.Notes != value)
                {
                    _model.Notes = value!;
                    OnPropertyChanged();
                }
            }
        }

        private ImageSource? _svgIcon;
        public ImageSource? SvgIcon
        {
            get => _svgIcon;
            private set
            {
                SetProperty(ref _svgIcon, value);
            }
        }

        public DateTime? DateAdded
        {
            get => _model.DateAdded!.Value.ToLocalTime();
            set
            {
                if (_model.DateAdded != value)
                {
                    _model.DateAdded = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? DateUpdate
        {
            get => _model.DateUpdate!.Value.ToLocalTime();
            set
            {
                if (_model.DateUpdate != value)
                {
                    _model.DateUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsFavorite
        {
            get => _model.IsFavorite;
            set
            {
                if (_model.IsFavorite != value)
                {
                    _model.IsFavorite = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ServiceNameFirstLetter
        {
            get
            {
                if (string.IsNullOrEmpty(ServiceName))
                    return "#";

                return ServiceName.Substring(0, 1).ToUpper();
            }
        }

        /// <summary>
        /// Устанавливает значение ID папки в null. Что бы отвязать ее от каких либо папок.
        /// </summary>
        public void SetDefaultFolderIdInfo()
        {
            IdFolder = null;
            FolderName = FolderViewModel.DISPLAYE_SECRETS_SYSTEM_FOLDER_NAME;
        }

        public void SetFolderName(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentNullException($"Название папки не может быть пустым.");

            FolderName = folderName;
        }

        /// <summary>
        /// Устанавливает нужное ID папки, в которой лежит запись. Вызывать после добавление записи в конкретную папку.
        /// </summary>
        /// <param name="folderId"></param>
        public void UpdateFolderID(int folderId)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(folderId, $"Значение {nameof(folderId)} вышло за допустимые пределы. Значение {nameof(folderId)} не может быть меньше 0.");

            IdFolder = folderId;
        }

        public void SetIcon(ImageSource svgIcon, string rawSvg)
        {
            SvgIcon = svgIcon;
            SetSvgCodeUnderlineModel(rawSvg);
        }

        private void SetSvgCodeUnderlineModel(string rawSvg) => _model.SvgIcon = rawSvg;

        public EncryptedSecret GetUnderlyingModel() => _model;
    }
}
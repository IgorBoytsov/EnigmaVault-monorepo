using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.FolderCase;
using EnigmaVault.Application.UseCases.Abstractions.IconCase;
using EnigmaVault.Application.UseCases.Abstractions.IconCategory;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.WPF.Client.Command;
using EnigmaVault.WPF.Client.Constants;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Models.Display;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace EnigmaVault.WPF.Client.ViewModels.Pages
{
    //TODO: Заменить все MessageBox.Show на кастомное окно
    internal sealed class UserSecretsPageVM : BasePageViewModel, IUpdatable, IAsyncInitializable
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ISecretsCryptoService _secretCryptoService;
        private readonly ICreateSecretUseCase _createSecretUseCase;
        private readonly IDeleteSecretUseCase _deleteSecretUseCase;
        private readonly IGetAllSecretsUseCase _getAllSecretsUseCase;

        private readonly IUpdateEncryptedDataUseCase _updateEncryptedDataUseCase;
        private readonly IUpdateFavoriteUseCase _updateFavoriteUseCase;
        private readonly IUpdateIsArchiveUseCase _updateIsArchiveUseCase;
        private readonly IUpdateMetaDataUseCase _updateMetaDataUseCase;
        private readonly IUpdateNoteUseCase _updateNoteUseCase;
        private readonly IUpdateFolderInSecretUseCase _updateFolderInSecretUseCase;
        private readonly IUpdateIconInSecretUseCase _updateIconInSecretUseCase;
        private readonly IUpdateSecretUseCase _updateSecretUseCase;

        private readonly ICreateFolderUseCase _createFolderUseCase;
        private readonly IGetAllFoldersUseCase _getAllFoldersUseCase;
        private readonly IUpdateFolderNameUseCase _updateFolderNameUseCase;
        private readonly IDeleteFolderUseCase _deleteFolderUseCase;

        private readonly IGetAllIconCategoryUseCase _getAllIconCategory;
        private readonly IGetAllIconsUseCase _getAllIconsUseCase;

        public UserSecretsPageVM(IAuthorizationService authorizationService,
                                 ISecretsCryptoService secretCryptoService,
                                 ICreateSecretUseCase createSecretUseCase,
                                 IDeleteSecretUseCase deleteSecretUseCase,
                                 IGetAllSecretsUseCase getAllSecretsUseCase,
                                 IUpdateEncryptedDataUseCase updateEncryptedDataUseCase,
                                 IUpdateFavoriteUseCase updateFavoriteUseCase,
                                 IUpdateIsArchiveUseCase updateIsArchiveUseCase,
                                 IUpdateMetaDataUseCase updateMetaDataUseCase, 
                                 IUpdateNoteUseCase updateNoteUseCase,
                                 IUpdateSecretUseCase updateSecretUseCase,
                                 IUpdateFolderInSecretUseCase updateFolderInSecretUseCase,
                                 IUpdateIconInSecretUseCase updateIconIsSecretUseCase,
                                 ICreateFolderUseCase createFolderUseCase,
                                 IGetAllFoldersUseCase getAllFoldersUseCase,
                                 IDeleteFolderUseCase deleteFolderUseCase,
                                 IUpdateFolderNameUseCase updateFolderNameUseCase,
                                 IGetAllIconCategoryUseCase getAllIconCategory,
                                 IGetAllIconsUseCase getAllIconsUseCase)
        {
            _authorizationService = authorizationService;
            _secretCryptoService = secretCryptoService;
            _createSecretUseCase = createSecretUseCase;
            _deleteSecretUseCase = deleteSecretUseCase;
            _getAllSecretsUseCase = getAllSecretsUseCase;

            _updateEncryptedDataUseCase = updateEncryptedDataUseCase;
            _updateFavoriteUseCase = updateFavoriteUseCase;
            _updateIsArchiveUseCase = updateIsArchiveUseCase;
            _updateMetaDataUseCase = updateMetaDataUseCase;
            _updateNoteUseCase = updateNoteUseCase;
            _updateFolderInSecretUseCase = updateFolderInSecretUseCase;
            _updateIconInSecretUseCase = updateIconIsSecretUseCase;
            _updateSecretUseCase = updateSecretUseCase;

            _createFolderUseCase = createFolderUseCase;
            _getAllFoldersUseCase = getAllFoldersUseCase;
            _updateFolderNameUseCase = updateFolderNameUseCase;
            _deleteFolderUseCase = deleteFolderUseCase;

            _getAllIconCategory = getAllIconCategory;
            _getAllIconsUseCase = getAllIconsUseCase;

            SecretsView = CollectionViewSource.GetDefaultView(_secrets);
            SecretsView.Filter = FilterSecrets;

            IconView = CollectionViewSource.GetDefaultView(_icons);

            SetVisibilityEditMenu(Visibility.Collapsed, 0);

            SetValueCommands();
            SetDefaultIcon();

            ArchivedSecrets.CollectionChanged += OnArchivedSecretsChanged!;
        }

        void IUpdatable.Update<TData>(TData value, TransmittingParameter parameter)
        {
            
        }

        async Task IAsyncInitializable.InitializeAsync()
        {
            if (IsInitialize)
                return;

            IsBusy = true;

            try
            {
                await GetSecrets();
                await GetFolders();

                SelectedGrouping = Grouping[0];
                SelectedSorting = Sorting[0];

                await GetIconCategories();
                await GetIcons();
                UpdateIconsView();

                IsInitialize = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        /*--Коллекции-------------------------------------------------------------------------------------*/

        private ObservableCollection<EncryptedSecretViewModel> _secrets { get; set; } = [];
        public ICollectionView SecretsView { get; private set; } = null!;

        public ObservableCollection<EncryptedSecretViewModel> ArchivedSecrets {  get; private set; } = [];

        private Dictionary<int, string> FolderLookup = [];
        public ObservableCollection<FolderViewModel> Folders { get; private set; } = [];

        private ObservableCollection<IconViewModel> _icons { get; set; } = [];
        public ICollectionView IconView { get; private set; } = null!;

        private List<IconCategoryDto> _iconCategories { get; set; } = [];

        public ObservableCollection<KeyValuePair<ViewGrouping, string>> Grouping { get; private set; } = new()
        {
            new KeyValuePair<ViewGrouping, string>(ViewGrouping.None, "Не применять"),
            new KeyValuePair<ViewGrouping, string>(ViewGrouping.Name, "Названию"),
            new KeyValuePair<ViewGrouping, string>(ViewGrouping.Alphabet, "Алфавиту"),
            new KeyValuePair<ViewGrouping, string>(ViewGrouping.Folder, "Папкам"),
        };
        
        public ObservableCollection<KeyValuePair<ViewSorting, string>> Sorting { get; private set; } = new()
        {
            new KeyValuePair<ViewSorting, string>(ViewSorting.Ascending, "По возрастанию (от А до Я, от 0 до 9)"),
            new KeyValuePair<ViewSorting, string>(ViewSorting.Descending, "По убыванию (от Я до А, от 9 до 0)"),
        };

        /*--Поля------------------------------------------------------------------------------------------*/

        #region SVG иконка

        private readonly Dictionary<string, DrawingImage> _iconCache = [];
        private DrawingImage? _defaultIcon;

        #endregion

        /*--Свойства--------------------------------------------------------------------------------------*/

        #region Selected

        private EncryptedSecretViewModel? _selectedSecretData;
        public EncryptedSecretViewModel? SelectedSecretData
        {
            get => _selectedSecretData;
            set
            {
                SetProperty(ref _selectedSecretData, value);

                CreateSecretCommand?.RaiseCanExecuteChanged();
                DeleteSecretCommand?.RaiseCanExecuteChanged();
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateMetadataCommand?.RaiseCanExecuteChanged();
                UpdateNoteCommand?.RaiseCanExecuteChanged();
                UpdateFavoriteCommand?.RaiseCanExecuteChanged();
                UpdateEncryptedDataCommand?.RaiseCanExecuteChanged();

                DecryptRecording(value!);

                SelectedIcon = _icons.FirstOrDefault(i => i.SvgCode == value?.SvgCode);
            }
        }

        private FolderViewModel? _selectedFolder;
        public FolderViewModel? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (SetProperty(ref _selectedFolder, value))
                {
                    FolderName = value?.FolderName;

                    if (value != null)
                        ViewFilterType = ViewFilterType.Folders;

                    SecretsView.Refresh();
                }
            }
        }         
        
        private IconViewModel? _selectedIcon;
        public IconViewModel? SelectedIcon
        {
            get => _selectedIcon;
            set
            {
                SetProperty(ref _selectedIcon, value);
            }
        }  

        private FolderViewModel? _selectedFolderInContextMenu;
        public FolderViewModel? SelectedFolderInContextMenu
        {
            get => _selectedFolderInContextMenu;
            set
            {
                SetProperty(ref _selectedFolderInContextMenu, value);
            }
        }

        private KeyValuePair<ViewGrouping, string> _selectedGrouping;
        public KeyValuePair<ViewGrouping, string> SelectedGrouping
        {
            get => _selectedGrouping;
            set
            {
                SetProperty(ref _selectedGrouping, value);
                UpdateSecretsView(value.Key, SelectedSorting.Key);
            }
        }
        
        private KeyValuePair<ViewSorting, string> _selectedSorting;
        public KeyValuePair<ViewSorting, string> SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                SetProperty(ref _selectedSorting, value);
                UpdateSecretsView(SelectedGrouping.Key, value.Key);
            }
        }

        private int _selectedTabControlIndex;
        public int SelectedTabControlIndex
        {
            get => _selectedTabControlIndex;
            set => SetProperty(ref _selectedTabControlIndex, value);
        }

        #endregion

        #region Общее кол-во записей

        private int _countAllSecrets;
        public int CountAllSecrets
        {
            get => _countAllSecrets;
            private set
            {
                SetProperty(ref _countAllSecrets, value);
            }
        }

        #endregion
        
        #region Общее кол-во записей в избранном

        private int _countFavoriteSecrets;
        public int CountFavoriteSecrets
        {
            get => _countFavoriteSecrets;
            private set
            {
                SetProperty(ref _countFavoriteSecrets, value);
            }
        }

        #endregion

        #region Текущая отображаемая фильтрация

        private ViewFilterType _viewFilterType = ViewFilterType.All;
        public ViewFilterType ViewFilterType
        {
            get => _viewFilterType;
            set
            {
                SetProperty(ref _viewFilterType, value);

                ShowAllSecretsCommand?.RaiseCanExecuteChanged();
                ShowFavoriteSecretsCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Расшифрованные данные

        private string? _nameSecret;
        public string? NameSecret
        {
            get => _nameSecret;
            set
            {
                SetProperty(ref _nameSecret, value);
                CreateSecretCommand?.RaiseCanExecuteChanged();
                UpdateMetadataCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _usernameSecret;
        public string? UsernameSecret
        {
            get => _usernameSecret;
            set
            {
                SetProperty(ref _usernameSecret, value);
                CreateSecretCommand?.RaiseCanExecuteChanged();
                UpdateEncryptedDataCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _emailSecret;
        public string? EmailSecret
        {
            get => _emailSecret;
            set
            {
                SetProperty(ref _emailSecret, value);
                CreateSecretCommand?.RaiseCanExecuteChanged();
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateEncryptedDataCommand?.RaiseCanExecuteChanged();
            }
        }
        
        private string? _recoveryKeysSecret;
        public string? RecoveryKeysSecret
        {
            get => _recoveryKeysSecret;
            set
            {
                SetProperty(ref _recoveryKeysSecret, value);
                CreateSecretCommand?.RaiseCanExecuteChanged();
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateEncryptedDataCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _secretWorldSecret;
        public string? SecretWorldSecret
        {
            get => _secretWorldSecret;
            set
            {
                SetProperty(ref _secretWorldSecret, value);
                CreateSecretCommand?.RaiseCanExecuteChanged();
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateEncryptedDataCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _passwordSecret;
        public string? PasswordSecret
        {
            get => _passwordSecret;
            set
            {
                SetProperty(ref _passwordSecret, value);
                CreateSecretCommand?.RaiseCanExecuteChanged();
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateEncryptedDataCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _urlSecret;
        public string? UrlSecret
        {
            get => _urlSecret;
            set
            {
                SetProperty(ref _urlSecret, value);
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateMetadataCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _notesSecret;
        public string? NotesSecret
        {
            get => _notesSecret;
            set 
            {
                SetProperty(ref _notesSecret, value);
                UpdateSecretCommand?.RaiseCanExecuteChanged();
                UpdateNoteCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Управление меню с добавлением\редактированием данных

        private Visibility _visibilityEditMenu;
        public Visibility VisibilityEditMenu
        {
            get => _visibilityEditMenu;
            set => SetProperty(ref _visibilityEditMenu, value);
        }

        private GridLength _editMenuColumnWidth = new GridLength(0);
        public GridLength EditMenuColumnWidth
        {
            get => _editMenuColumnWidth;
            set => SetProperty(ref _editMenuColumnWidth, value);
        }

        private int _minWidthEditMenu;
        public int MinWidthEditMenu
        {
            get => _minWidthEditMenu;
            set => SetProperty(ref _minWidthEditMenu, value);
        }

        #endregion

        #region Название папки

        private string? _folderName;
        public string? FolderName
        {
            get => _folderName;
            set
            {
                SetProperty(ref _folderName, value);

                CreateFolderCommand?.RaiseCanExecuteChanged();
                UpdateFolderNameCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Переключение вида отображаемых данных

        private TemplateType _selectedTemplate = TemplateType.Detailed;
        public TemplateType SelectedTemplate
        {
            get => _selectedTemplate;
            set 
            {
                SetProperty(ref _selectedTemplate, value);
                SwitchTemplateSecretsCommand?.RaiseCanExecuteChanged();
            } 
        }

        #endregion

        /*---Popup---*/

        #region Popup : ArchivePopup

        private bool _isArchivePopupOpen;
        public bool IsArchivePopupOpen
        {
            get => _isArchivePopupOpen;
            set
            {
                if (value && IsArchiveEmpty)
                    return;

                if (SetProperty(ref _isArchivePopupOpen, value))
                    _isArchivePopupOpen = value;
            }
        }

        public bool IsArchiveEmpty => ArchivedSecrets == null || ArchivedSecrets.Count == 0;

        #endregion 

        /*--Команды---------------------------------------------------------------------------------------*/

        private void SetValueCommands()
        {
            CreateSecretCommand = new RelayCommandAsync(Execute_CreateSecretCommand, CanExecute_CreateSecretCommand);
            DeleteSecretCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_DeleteSecretCommand, CanExecute_DeleteSecretCommand);

            UpdateSecretCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateSecretCommand, CanExecute_UpdateSecretCommand);
            UpdateEncryptedDataCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateEncryptedDataCommand, CanExecute_UpdateEncryptedDataCommand);
            UpdateMetadataCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateMetadataCommand, CanExecute_UpdateMetadataCommand);
            UpdateFavoriteCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateFavoriteCommand, CanExecute_UpdateFavoriteCommand);
            UpdateArchiveCommand = new RelayCommand<EncryptedSecretViewModel>(Execute_UpdateArchiveCommand, CanExecute_UpdateArchiveCommand);
            UpdateNoteCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateNoteCommand, CanExecute_UpdateNoteCommand);
            UpdateIconSecretCommand = new RelayCommand<IconViewModel>(Execute_UpdateIconSecretCommand, CanExecute_UpdateIconSecretCommand);
            RemoveSecretFromFolder = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_RemoveSecretFromFolder, CanExecute_RemoveSecretFromFolder);

            CreateFolderCommand = new RelayCommandAsync(Execute_CreateFolderCommand, CanExecute_CreateFolderCommand);
            UpdateFolderNameCommand = new RelayCommandAsync<FolderViewModel>(Execute_UpdateFolderNameCommand, CanExecute_UpdateFolderNameCommand);
            DeleteFolderCommand = new RelayCommandAsync<FolderViewModel>(Execute_DeleteFolderCommand, CanExecute_DeleteFolderCommand);
            AddSecretInFolder = new RelayCommandAsync<FolderViewModel>(Execute_AddSecretInFolder, CanExecute_AddSecretInFolder);

            OpenUrlCommand = new RelayCommand<string>(Execute_OpenUrlCommand, CanExecute_OpenUrlCommand);
            SwitchTemplateSecretsCommand = new RelayCommand<TemplateType>(Execute_SwitchTemplateSecretsCommand, CanExecute_SwitchTemplateSecretsCommand);

            ShowEditMenuCommand = new RelayCommand<EncryptedSecretViewModel>(Execute_ShowEditMenuCommand, CanExecute_ShowEditMenuCommand);
            HideEditMenuCommand = new RelayCommand(Execute_HideEditMenuCommand, CanExecute_HideEditMenuCommand);

            ShowAllSecretsCommand = new RelayCommand(Execute_ShowAllSecretsCommand, CanExecute_ShowAllSecretsCommand);
            ShowIconsListCommand = new RelayCommand(Execute_ShowIconsListCommand, CanExecute_ShowIconsListCommand);
            ShowFavoriteSecretsCommand = new RelayCommand(Execute_ShowFavoriteSecretsCommand, CanExecute_ShowFavoriteSecretsCommand);
        }

        /*--Create--*/

        #region [CreateSecretCommand] - Добавление записи в систему

        public RelayCommandAsync? CreateSecretCommand { get; private set; }

        private async Task Execute_CreateSecretCommand()
        {
            var result = await _createSecretUseCase.Create(NameSecret!, UsernameSecret!, PasswordSecret!, EmailSecret!, SecretWorldSecret!, RecoveryKeysSecret!, UrlSecret, NotesSecret, false);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            var newSecret = new EncryptedSecretViewModel(result.Value!);

            _secrets.Add(newSecret);

            newSecret.SetFolderName(FolderViewModel.DISPLAYE_SECRETS_SYSTEM_FOLDER_NAME); // Установить названия папки для отображение у секрета в списке.
            newSecret.SetIcon(_defaultIcon!, DefaultIconConstants.DEFOULT_SECRET_SVG);
            CalculateElementsInfFolder();

            SecretsView.Refresh();
        }

        private bool CanExecute_CreateSecretCommand() 
            => !string.IsNullOrWhiteSpace(NameSecret) || !string.IsNullOrWhiteSpace(UsernameSecret) || !string.IsNullOrWhiteSpace(PasswordSecret) || !string.IsNullOrWhiteSpace(EmailSecret);

        #endregion

        #region [CreateFolderCommand] - Создание папки

        public RelayCommandAsync? CreateFolderCommand { get; private set; }

        private async Task Execute_CreateFolderCommand()
        {
            if (Folders.Any(f => f.FolderName == FolderName))
            {
                MessageBox.Show("Папка с таким названием уже существует.");
                return;
            }

            var result = await _createFolderUseCase.CreateAsync(_authorizationService.CurrentUser!.IdUser, FolderName!);

            if (!result.IsSuccess)
                MessageBox.Show(string.Join(';', result.Errors));

            Folders.Add(new FolderViewModel(result.Value!));
            FolderLookup.TryAdd(result.Value!.IdFolder, result.Value!.FolderName);
        }

        private bool CanExecute_CreateFolderCommand()
        {
            if (string.IsNullOrWhiteSpace(FolderName)) return false;
            if (SelectedFolder?.FolderName.ToLower() == FolderName?.ToLower()) return false;

            string trimmedNewName = FolderName!.Trim();

            if (Folders.Any(f => string.Equals(f.FolderName?.Trim(), trimmedNewName, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }

        #endregion

        /*----Add----*/

        #region [AddSecretInFolder] - Добавить запись в папку

        public RelayCommandAsync<FolderViewModel>? AddSecretInFolder { get; private set; }

        private async Task Execute_AddSecretInFolder(FolderViewModel folder)
        {
            if (SelectedSecretData is null)
            {
                MessageBox.Show("Вы не выбрали запись.");
                return;
            }

            var result = await _updateFolderInSecretUseCase.UpdateFolderAsync(SelectedSecretData.IdSecret, folder.IdFolder);

            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Errors.ToString());
                return;
            }

            var newFolder = folder;
            var oldFolder = Folders.FirstOrDefault(f => f.IdFolder == SelectedSecretData.IdFolder);

            SelectedSecretData.UpdateFolderID(folder.IdFolder);
            SelectedSecretData.SetFolderName(folder.FolderName);
      
            newFolder?.SetCountElements(_secrets.Count(s => s.IdFolder == newFolder.IdFolder));

            if (oldFolder != null)
                oldFolder!.SetCountElements(_secrets.Count(s => s.IdFolder == oldFolder.IdFolder));

            SecretsView.Refresh();
        }

        private bool CanExecute_AddSecretInFolder(FolderViewModel folder) => folder != null && SelectedSecretData != null;

        #endregion

        /*--Delete--*/

        #region [DeleteSecretCommand] - Удаление записи из системы

        public RelayCommandAsync<EncryptedSecretViewModel>? DeleteSecretCommand { get; private set; }

        private async Task Execute_DeleteSecretCommand(EncryptedSecretViewModel secret)
        {
            if (secret.IsArchive)
            {
                MessageBox.Show("Нельзя удалить архивированную запись");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить данные: {secret.ServiceName}?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            var result = await _deleteSecretUseCase.DeleteAsync(secret.IdSecret);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            int? currentSecretFolderID = secret.IdFolder;

            _secrets.Remove(secret);
            CalculateElementsInfFolder(currentSecretFolderID);
            CalculateElementsInfFolder();
        }

        private bool CanExecute_DeleteSecretCommand(EncryptedSecretViewModel secret) => secret != null;

        #endregion

        #region [RemoveSecretFromFolder] - Убрать запись из папки ( Переноситься в общую категорию )

        public RelayCommandAsync<EncryptedSecretViewModel>? RemoveSecretFromFolder {  get; private set; }

        private async Task Execute_RemoveSecretFromFolder(EncryptedSecretViewModel encryptedSecret)
        {
            var result = await _updateFolderInSecretUseCase.UpdateFolderAsync(encryptedSecret.IdSecret, null);

            if (!result.IsSuccess)
            {
                var sb = new StringBuilder();

                foreach (var error in result.Errors)
                    sb.Append(error.Description);

                MessageBox.Show(sb.ToString());
                return;
            }
            var idOldFolder = encryptedSecret.IdFolder;

            encryptedSecret.SetDefaultFolderIdInfo();
            CalculateElementsInfFolder(idOldFolder);

            SecretsView.Refresh();
        }

        private bool CanExecute_RemoveSecretFromFolder(EncryptedSecretViewModel encryptedSecret) => true;

        #endregion

        #region [DeleteFolderCommand] - Удаление папики

        public RelayCommandAsync<FolderViewModel>? DeleteFolderCommand {  get; private set; }

        private async Task Execute_DeleteFolderCommand(FolderViewModel folder)
        {
            if (folder.IdFolder == -1)
            {
                MessageBox.Show("Нельзя удалить данную папку.");
                return;
            }

            if (folder is null)
            {
                MessageBox.Show("Вы не выбрали папку для удаления.");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите удалить папку: {folder.FolderName}?", "Предупреждение", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            var result = await _deleteFolderUseCase.DeleteAsync(folder.IdFolder);

            if (!result.IsSuccess)
            {
                var sb = new StringBuilder();

                foreach (var error in result.Errors)
                    sb.Append(error.Description);

                MessageBox.Show(sb.ToString());
                return;
            }
                
            var secretsInFolder = _secrets.Where(s => s.IdFolder == folder.IdFolder).ToList();

            foreach (var item in secretsInFolder)
                item.SetDefaultFolderIdInfo();

            Folders.Remove(folder);
        }

        private bool CanExecute_DeleteFolderCommand(FolderViewModel folder) => true;

        #endregion

        /*--Update--*/

        #region [UpdateSecretCommand] - Обновление всех данных

        public RelayCommandAsync<EncryptedSecretViewModel>? UpdateSecretCommand { get; private set; }

        private async Task Execute_UpdateSecretCommand(EncryptedSecretViewModel secret)
        {
            var decrypted = new DecryptedSecret
            {
                IdSecret = secret.IdSecret,
                DateAdded = secret.DateAdded,
                DateUpdate = secret.DateUpdate,      
                SchemaVersion = secret.SchemaVersion,
                IsFavorite = secret.IsFavorite,

                ServiceName = NameSecret!,
                Url = UrlSecret,
                Notes = NotesSecret,
                Username = UsernameSecret!,
                Password = PasswordSecret!,
                Email = EmailSecret!,
                RecoveryKeys = RecoveryKeysSecret,
                SecretWord = SecretWorldSecret,   
            };

            var result = await _updateSecretUseCase.UpdateSecretAsync(decrypted);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            secret.ServiceName = decrypted.ServiceName;
            secret.Url = decrypted.Url;
            secret.EncryptedData = result.Value.EncryptedData;
            secret.Nonce = result.Value.Nonce;
            secret.IsFavorite = decrypted.IsFavorite;
            secret.Notes = decrypted.Notes;
            secret.DateUpdate = result.Value.DateTime!.Value.ToLocalTime();

            SelectedSecretData = secret;

            OnPropertyChanged(nameof(SelectedSecretData));

            SecretsView.Refresh();
        }

        private bool CanExecute_UpdateSecretCommand(EncryptedSecretViewModel secret) => secret != null;

        #endregion

        #region [UpdateEncryptedDataCommand] - Обновление чувствительных данных

        public RelayCommandAsync<EncryptedSecretViewModel>? UpdateEncryptedDataCommand { get; private set; }

        private async Task Execute_UpdateEncryptedDataCommand(EncryptedSecretViewModel secret)
        {
            var sensitive = new SensitiveSecretData
            {
                Username = UsernameSecret!,
                Password = PasswordSecret!,
                Email = EmailSecret!,
                RecoveryKeys = RecoveryKeysSecret,
                SecretWord = SecretWorldSecret,
            };

            var result = await _updateEncryptedDataUseCase.UpdateEncryptedDataAsync(secret.IdSecret, sensitive);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            secret.EncryptedData = result.Value.EncryptedData;
            secret.Nonce = result.Value.Nonce;
            secret.DateUpdate = result.Value.DateUpdated.ToLocalTime();
        }

        private bool CanExecute_UpdateEncryptedDataCommand(EncryptedSecretViewModel secret) 
            => secret is not null && (!string.IsNullOrWhiteSpace(UsernameSecret) || !string.IsNullOrWhiteSpace(PasswordSecret) || !string.IsNullOrWhiteSpace(EmailSecret));

        #endregion

        #region [UpdateMetadataCommand] - Обновление мета данных

        public RelayCommandAsync<EncryptedSecretViewModel>? UpdateMetadataCommand { get; private set; }

        private async Task Execute_UpdateMetadataCommand(EncryptedSecretViewModel secret)
        {
            var result = await _updateMetaDataUseCase.UpdateMetaDataAsync(secret.IdSecret, NameSecret!, UrlSecret);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            secret.ServiceName = NameSecret!;
            secret.Url = UrlSecret;
            secret.DateUpdate = result.Value.ToLocalTime();

            OnPropertyChanged(nameof(SelectedSecretData));

            SecretsView.Refresh();
        }

        private bool CanExecute_UpdateMetadataCommand(EncryptedSecretViewModel secret)
           => secret is not null && !string.IsNullOrWhiteSpace(NameSecret);

        #endregion

        #region [UpdateFavoriteCommand] - Обновление статуса "Избранное"

        public RelayCommandAsync<EncryptedSecretViewModel>? UpdateFavoriteCommand { get; private set; }

        private async Task Execute_UpdateFavoriteCommand(EncryptedSecretViewModel secret)
        {
            var newFavoriteState = !secret.IsFavorite;
            secret.IsFavorite = newFavoriteState;

            var result = await _updateFavoriteUseCase.UpdateFavoriteAsync(secret.IdSecret, newFavoriteState);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);

                secret.IsFavorite = !newFavoriteState;
                return;
            }

            secret.DateUpdate = result.Value.ToLocalTime();
            OnPropertyChanged(nameof(SelectedSecretData));

            CalculateFavoriteElements();
        }

        private bool CanExecute_UpdateFavoriteCommand(EncryptedSecretViewModel secret) => secret is not null;

        #endregion

        #region [UpdateArchiveCommand] - Добавление \ удаление в архив

        public RelayCommand<EncryptedSecretViewModel>? UpdateArchiveCommand { get; private set; }

        private async void Execute_UpdateArchiveCommand(EncryptedSecretViewModel secret)
        {
            var newArchiveState = !secret.IsArchive;
            secret.IsArchive = newArchiveState;

            var result = await _updateIsArchiveUseCase.UpdateArchiveAsync(secret.IdSecret, newArchiveState);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);

                secret.IsArchive = !newArchiveState;
                return;
            }

            if (secret.IsArchive)
            {
                ArchivedSecrets.Add(secret);
                _secrets.Remove(secret);
            }
            else
            {
                _secrets.Add(secret);
                ArchivedSecrets.Remove(secret);
            }

            CalculateElementsInfFolder();
            CalculateElementsInfFolder(secret.IdFolder);
            CalculateFavoriteElements();
        }

        private bool CanExecute_UpdateArchiveCommand(EncryptedSecretViewModel secret) => secret is not null;

        #endregion

        #region [UpdateNoteCommand] - Обновление заметки

        public RelayCommandAsync<EncryptedSecretViewModel>? UpdateNoteCommand { get; private set; }

        private async Task Execute_UpdateNoteCommand(EncryptedSecretViewModel secret)
        {
            var result = await _updateNoteUseCase.UpdateNoteAsync(secret.IdSecret, NotesSecret);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            secret.Notes = NotesSecret;
            secret.DateUpdate = result.Value.ToLocalTime();

            OnPropertyChanged(nameof(SelectedSecretData));
        }

        private bool CanExecute_UpdateNoteCommand(EncryptedSecretViewModel secret) => secret is not null;

        #endregion

        #region [UpdateFolderNameCommand] - Обновление названия папки

        public RelayCommandAsync<FolderViewModel>? UpdateFolderNameCommand { get; private set; }

        private async Task Execute_UpdateFolderNameCommand(FolderViewModel folder)
        {
            var result = await _updateFolderNameUseCase.UpdateNameAsync(folder.IdFolder, FolderName!);

            if (!result.IsSuccess)
            {
                var sb = new StringBuilder();

                foreach (var item in result.Errors)
                    sb.Append(item.Description);

                MessageBox.Show(sb.ToString());
                return;
            }

            RenameFolderNameInSecrets(folder, FolderName!);
        }

        private bool CanExecute_UpdateFolderNameCommand(FolderViewModel folder)
        {
            if (folder is null) return false;
            if (string.IsNullOrWhiteSpace(FolderName)) return false;
            if (folder?.FolderName.ToLower() == FolderName?.ToLower()) return false;

            string trimmedNewName = FolderName!.Trim();

            if (Folders.Any(f => string.Equals(f.FolderName?.Trim(), trimmedNewName, StringComparison.OrdinalIgnoreCase)))
                return false;

            return true;
        }

        #endregion

        #region [UpdateIconSecretCommand] - Обновление иконки секрета

        public RelayCommand<IconViewModel>? UpdateIconSecretCommand { get; private set; }

        private async void Execute_UpdateIconSecretCommand(IconViewModel icon)
        {
            var result = await _updateIconInSecretUseCase.UpdateAsync(SelectedSecretData!.IdSecret, icon.SvgCode);

            if (!result.IsSuccess)
            {
                var sb = new StringBuilder();

                foreach (var item in result.Errors)
                    sb.Append(item.Description);

                MessageBox.Show(sb.ToString());
                return;
            }

            SelectedSecretData?.SetIcon(icon!.SvgIcon!, icon.SvgCode);
        }

        private bool CanExecute_UpdateIconSecretCommand(IconViewModel icon) => SelectedSecretData != null;

        #endregion

        /*--Shared--*/

        #region [OpenUrlCommand] - Переход по ссылки

        public RelayCommand<string>? OpenUrlCommand { get; private set; }

        private void Execute_OpenUrlCommand(string url)
        {
            try
            {
                var psi = new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Не удалось открыть ссылку: {ex.Message}");
                MessageBox.Show($"Не удаётся перенаправить на сайт: {url}. По причине {ex.Message}");
            }
        }

        private bool CanExecute_OpenUrlCommand(string url) => !string.IsNullOrEmpty(url);

        #endregion

        #region [SwitchTemplateSecretsCommand] - Переключение шаблона списка секретов

        public RelayCommand<TemplateType>? SwitchTemplateSecretsCommand { get; private set; }

        private void Execute_SwitchTemplateSecretsCommand(TemplateType type) => SelectedTemplate = type;

        private bool CanExecute_SwitchTemplateSecretsCommand(TemplateType type) => type != SelectedTemplate;

        #endregion

        /*--Управление EditMenu--*/
        
        #region [ShowEditMenuCommand] - Показать меню редактирование

        public RelayCommand<EncryptedSecretViewModel>? ShowEditMenuCommand { get; private set; }

        private void Execute_ShowEditMenuCommand(EncryptedSecretViewModel secret) 
        {
            SetVisibilityEditMenu(Visibility.Visible, 270);

            EditMenuColumnWidth = new GridLength(1, GridUnitType.Star);
            SelectedTabControlIndex = 0;

            if (secret is null)
                SetNulEditFields();
            else
                SelectedSecretData = secret;

        } 

        private bool CanExecute_ShowEditMenuCommand(EncryptedSecretViewModel secret) => true;

        #endregion

        #region [ShowIconsListCommand] - Открыть меню с заменой иконок

        public RelayCommand? ShowIconsListCommand { get; private set; }

        private void Execute_ShowIconsListCommand()
        {
            SetVisibilityEditMenu(Visibility.Visible, 270);

            EditMenuColumnWidth = new GridLength(1, GridUnitType.Star);
            SelectedTabControlIndex = 1;
        }

        private bool CanExecute_ShowIconsListCommand() => true;

        #endregion

        #region [HideEditMenuCommand] - Скрыть меню редактирование

        public RelayCommand? HideEditMenuCommand { get; private set; }

        private void Execute_HideEditMenuCommand()
        {
            SetVisibilityEditMenu(Visibility.Collapsed, 0);
            EditMenuColumnWidth = new GridLength(0);
        } 

        private bool CanExecute_HideEditMenuCommand() 
            => VisibilityEditMenu != Visibility.Collapsed || VisibilityEditMenu != Visibility.Hidden;

        #endregion

        /*--Команды сортировки \ фильтрации--*/

        #region [ShowAllSecretsCommand] - Показать список всех секретов пользователя

        public RelayCommand? ShowAllSecretsCommand {  get; private set; }

        private void Execute_ShowAllSecretsCommand()
        {
            SelectedFolder = null;
            ViewFilterType = ViewFilterType.All;
            SecretsView.Refresh();
        }

        private bool CanExecute_ShowAllSecretsCommand() => ViewFilterType != ViewFilterType.All;

        #endregion

        #region [ShowFavoriteSecretsCommand] - Показать все избранные секреты

        public RelayCommand? ShowFavoriteSecretsCommand {  get; private set; }

        private void Execute_ShowFavoriteSecretsCommand()
        {
            ViewFilterType = ViewFilterType.Favorite;

            SecretsView.Refresh();
        }

        private bool CanExecute_ShowFavoriteSecretsCommand() => ViewFilterType != ViewFilterType.Favorite;

        #endregion

        /*--Методы----------------------------------------------------------------------------------------*/

        /*--Получение данных--*/

        #region Secrets

        private async Task GetSecrets()
        {
            if (_authorizationService.CurrentUser is null)
                throw new Exception("Пользователь не был залогинен");

            var result = await _getAllSecretsUseCase.GetSecretsAsync(_authorizationService.CurrentUser.IdUser);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                //TODO: Заменить MessageBox.Show на кастомное окно
                MessageBox.Show(errors);
                return;
            }

            foreach (var item in result.Value!)
            {
                var icon = await Task.Run(() => ConvertSVG(item.SvgIcon));

                var display = new EncryptedSecretViewModel(item);
                display.SetIcon(icon!, item.SvgIcon);

                (display.IsArchive ? ArchivedSecrets : _secrets).Add(display);
            }
                
            CalculateFavoriteElements();
        }

        #endregion

        #region Folders

        private async Task GetFolders()
        {
            if (_authorizationService.CurrentUser is null)
                throw new Exception("Пользователь не был залогинен");

            var result = await _getAllFoldersUseCase.GetAllAsync(_authorizationService.CurrentUser.IdUser);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            FolderLookup = result.Value!.ToDictionary(f => f.IdFolder, f => f.FolderName);

            RenameFolderName();
            LoadFolders(result.Value!);
            CalculateElementsInfFolder();
        }

        private void RenameFolderName()
        {
            foreach (var secret in _secrets)
            {
                if (secret.IdFolder.HasValue && FolderLookup.TryGetValue(secret.IdFolder.Value, out var folderName))
                    secret.SetFolderName(folderName);
                else
                    secret.SetFolderName(FolderViewModel.DISPLAYE_SECRETS_SYSTEM_FOLDER_NAME);
            }
        }

        private void LoadFolders(List<FolderDto> list)
        {
            foreach (var item in list)
            {
                var displayedItem = new FolderViewModel(item);
                displayedItem.SetCountElements(_secrets.Count(s => s.IdFolder == item.IdFolder));

                Folders.Add(displayedItem);
            }
        }

        #endregion

        #region Icons

        private async Task GetIcons()
        {
            var result = await _getAllIconsUseCase.GetAllAsync(_authorizationService.CurrentUser!.IdUser);
            
            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            foreach (var item in result.Value!)
            {
                var icon = await Task.Run(() => ConvertSVG(item.SvgCode));
                var categoryName = _iconCategories.FirstOrDefault(c => c.IdCategory == item.IdIconCategory)?.Name;

                var display = new IconViewModel(item);
                display.SetIcon(icon!);
                display.SetCategoryName(categoryName ??= "#");

                _icons.Add(display);
            }
        }

        #endregion

        #region IconCategories

        private async Task GetIconCategories()
        {
            var result = await _getAllIconCategory.GetAllAsync();

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            foreach (var item in result.Value!)
                _iconCategories.Add(item);
        }

        #endregion

        /*--Фильтрация, Группировка, Сортировка CollectionView--*/

        #region SecretsView

        private bool FilterSecrets(object item)
        {
            if (item is not EncryptedSecretViewModel record)
                return false;

            return ViewFilterType switch
            {
                ViewFilterType.All => true,
                ViewFilterType.Favorite => record.IsFavorite,
                ViewFilterType.Folders => SelectedFolder != null && record.IdFolder == SelectedFolder.IdFolder,
                _ => true,
            };
        }

        private void UpdateSecretsView(ViewGrouping grouping, ViewSorting sorting)
        {
            SecretsView.GroupDescriptions.Clear();
            SecretsView.SortDescriptions.Clear();
            ListCollectionView? lcv = null;

            if (SecretsView is ListCollectionView listCollectionView)
            {
                lcv = listCollectionView;
                lcv.CustomSort = null;
            }

            var direction = sorting == ViewSorting.Ascending
                ? ListSortDirection.Ascending
                : ListSortDirection.Descending;

            Action? setupAction = grouping switch
            {
                ViewGrouping.None => () => SecretsView.SortDescriptions.Add(new SortDescription("ServiceName", direction)),
                ViewGrouping.Folder => () =>
                {
                    SecretsView.GroupDescriptions.Add(new PropertyGroupDescription("FolderName"));

                    SecretsView.SortDescriptions.Add(new SortDescription("FolderName", direction));
                    SecretsView.SortDescriptions.Add(new SortDescription("ServiceName", direction));
                }
                ,
                ViewGrouping.Alphabet => () =>
                {
                    SecretsView.GroupDescriptions.Add(new PropertyGroupDescription("ServiceNameFirstLetter"));
                    SecretsView.SortDescriptions.Add(new SortDescription("ServiceName", direction));
                }
                ,
                ViewGrouping.Name => () =>
                {
                    SecretsView.GroupDescriptions.Add(new PropertyGroupDescription("ServiceName"));
                    SecretsView.SortDescriptions.Add(new SortDescription("ServiceName", direction));
                }
                ,
                _ => null
            };

            setupAction?.Invoke();
        }

        #endregion

        #region IconsView
 
        private void UpdateIconsView()
        {
            IconView.SortDescriptions.Clear();
            IconView.GroupDescriptions.Clear();

            IconView.SortDescriptions.Add(new SortDescription("CategoryName", ListSortDirection.Ascending));
            IconView.GroupDescriptions.Add(new PropertyGroupDescription("CategoryName"));
        }

        #endregion

        /*--Вспомогательные--*/

        #region Подсчет кол-ва элементов

        /// <summary>
        /// Передать null если нужно установить дефолтное ID для системной папки.
        /// </summary>
        /// <param name="idFolder"></param>
        private void CalculateElementsInfFolder(int? idFolder = null)
        {
            if (!idFolder.HasValue)
            {
                CountAllSecrets = _secrets.Count;
                return;
            }

            Folders.FirstOrDefault(f => f.IdFolder == idFolder.Value)!.SetCountElements(_secrets.Count(s => s.IdFolder == idFolder));
        }

        private void CalculateFavoriteElements() => CountFavoriteSecrets = _secrets.Count(s => s.IsFavorite == true);

        #endregion

        #region Переименовка название папки у записей, при изменение название папки

        private void RenameFolderNameInSecrets(FolderViewModel folder, string newName)
        {
            try
            {
                folder.SetName(newName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            foreach (var secret in _secrets.Where(s => s.IdFolder == folder.IdFolder))
                secret.SetFolderName(newName);
        }

        #endregion

        #region Расшифровка выбранной записи

        private void DecryptRecording(EncryptedSecretViewModel secret)
        {
            if (secret is null)
                return;

            var decrypted = _secretCryptoService.Decrypt(secret.GetUnderlyingModel());

            NameSecret = secret.ServiceName;

            UsernameSecret = decrypted.Username;
            PasswordSecret = decrypted.Password;
            EmailSecret = decrypted.Email;
            RecoveryKeysSecret = decrypted.RecoveryKeys;
            SecretWorldSecret = decrypted.SecretWord;

            UrlSecret = secret.Url;
            NotesSecret = secret.Notes;
        }

        #endregion

        #region Преобразование Svg

        private DrawingImage? ConvertSVG(string? svgCode)
        {
            ArgumentNullException.ThrowIfNull(_defaultIcon, nameof(_defaultIcon));

            if (string.IsNullOrWhiteSpace(svgCode))
                return _defaultIcon;

            if (_iconCache.TryGetValue(svgCode, out var cachedIcon))
                return cachedIcon;

            var svgString = ReplaceDoubleQuotesWithSingle(svgCode)!;
            var newIcon = ConvertSvgInString(svgString);

            if (newIcon is null)
                return _defaultIcon!;

            _iconCache[svgCode] = newIcon;
            return newIcon;
        }

        private static DrawingImage? ConvertSvgInString(string svgCode)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = true
            };

            using (var stringReader = new StringReader(svgCode))
            {
                var reader = new FileSvgReader(settings);
                DrawingGroup drawingGroup = reader.Read(stringReader);

                if (drawingGroup != null)
                {
                    var drawingImage = new DrawingImage(drawingGroup);

                    drawingImage.Freeze();

                    return drawingImage;
                }

                return null;
            }
        }

        private void SetDefaultIcon()
        {
            if (_defaultIcon is not null) return;

            var defaultIconImage = ConvertSvgInString(DefaultIconConstants.DEFOULT_SECRET_SVG) ?? throw new InvalidOperationException("Не удалось создать дефолтную иконку.");

            _defaultIcon = defaultIconImage;

            // _iconCache[DefaultIconConstants.DEFOULT_SECRET_SVG] = _defaultIcon;
        }

        private static string? ReplaceDoubleQuotesWithSingle(string inputString) => inputString?.Replace("\"", "'");

        #endregion

        /*--UI--*/

        #region Сброс полей для добавление\редактирование данных

        private void SetNulEditFields()
        {
            NameSecret = string.Empty;
            UsernameSecret = string.Empty;
            PasswordSecret = string.Empty;
            EmailSecret = string.Empty;
            RecoveryKeysSecret = string.Empty;
            SecretWorldSecret = string.Empty;
            UrlSecret = string.Empty;
            NotesSecret = string.Empty;
            SelectedSecretData = null;
        }

        #endregion

        #region Отображение меню изменение и добавление данных

        private void SetVisibilityEditMenu(Visibility visibility, int minWidthEditMenu)
        {
            VisibilityEditMenu = visibility;
            EditMenuColumnWidth = new GridLength(minWidthEditMenu);
            MinWidthEditMenu = minWidthEditMenu;
        }

        #endregion

        /*--Методы (OnChanged)----------------------------------------------------------------------------*/

        #region Коллекции

        private void OnArchivedSecretsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsArchiveEmpty));

            if (IsArchiveEmpty && IsArchivePopupOpen)
                IsArchivePopupOpen = false;
        }

        #endregion

    }
}
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.WPF.Client.Command;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Models.Display;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.ViewModels.Pages
{
    //TODO: Заменить все MessageBox.Show на кастомное окно
    internal class UserSecretsPageVM : BasePageViewModel, IUpdatable
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ISecretsCryptoService _secretCryptoService;
        private readonly ICreateSecretUseCase _createSecretUseCase;
        private readonly IDeleteSecretUseCase _deleteSecretUseCase;
        private readonly IGetAllSecretsUseCase _getAllSecretsUseCase;

        private readonly IUpdateEncryptedDataUseCase _updateEncryptedDataUseCase;
        private readonly IUpdateFavoriteUseCase _updateFavoriteUseCase;
        private readonly IUpdateMetaDataUseCase _updateMetaDataUseCase;
        private readonly IUpdateNoteUseCase _updateNoteUseCase;
        private readonly IUpdateSecretUseCase _updateSecretUseCase;

        public UserSecretsPageVM(IAuthorizationService authorizationService,
                                 ISecretsCryptoService secretCryptoService,
                                 ICreateSecretUseCase createSecretUseCase,
                                 IDeleteSecretUseCase deleteSecretUseCase,
                                 IGetAllSecretsUseCase getAllSecretsUseCase,
                                 IUpdateEncryptedDataUseCase updateEncryptedDataUseCase,
                                 IUpdateFavoriteUseCase updateFavoriteUseCase,
                                 IUpdateMetaDataUseCase updateMetaDataUseCase, 
                                 IUpdateNoteUseCase updateNoteUseCase,
                                 IUpdateSecretUseCase updateSecretUseCase)
        {
            _authorizationService = authorizationService;
            _secretCryptoService = secretCryptoService;
            _createSecretUseCase = createSecretUseCase;
            _deleteSecretUseCase = deleteSecretUseCase;
            _getAllSecretsUseCase = getAllSecretsUseCase;

            _updateEncryptedDataUseCase = updateEncryptedDataUseCase;
            _updateFavoriteUseCase = updateFavoriteUseCase;
            _updateMetaDataUseCase = updateMetaDataUseCase;
            _updateNoteUseCase = updateNoteUseCase;
            _updateSecretUseCase = updateSecretUseCase;

            SetVisibilityEditMenu(Visibility.Collapsed, 0);

            SetValueCommands();

            GetSecrets();
        }

        public void Update<TData>(TData value, TransmittingParameter parameter = TransmittingParameter.None)
        {
            
        }

        /*--Коллекции-------------------------------------------------------------------------------------*/

        public ObservableCollection<EncryptedSecretViewModel> SecretDatas { get; private set; } = [];

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

        //private bool _isFavorite;
        //public bool IsFavorite
        //{
        //    get => _isFavorite;
        //    set
        //    {
        //        SetProperty(ref _isFavorite, value);
        //        UpdateSecretCommand?.RaiseCanExecuteChanged();
        //        UpdateFavoriteCommand?.RaiseCanExecuteChanged();
        //    }
        //}


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

        /*--Команды---------------------------------------------------------------------------------------*/

        private void SetValueCommands()
        {
            CreateSecretCommand = new RelayCommandAsync(Execute_SaveSecretCommand, CanExecute_SaveSecretCommand);
            DeleteSecretCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_DeleteSecretCommand, CanExecute_DeleteSecretCommand);

            UpdateSecretCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateSecretCommand, CanExecute_UpdateSecretCommand);
            UpdateEncryptedDataCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateEncryptedDataCommand, CanExecute_UpdateEncryptedDataCommand);
            UpdateMetadataCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateMetadataCommand, CanExecute_UpdateMetadataCommand);
            UpdateFavoriteCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateFavoriteCommand, CanExecute_UpdateFavoriteCommand);
            UpdateNoteCommand = new RelayCommandAsync<EncryptedSecretViewModel>(Execute_UpdateNoteCommand, CanExecute_UpdateNoteCommand);

            OpenUrlCommand = new RelayCommand<string>(Execute_OpenUrlCommand, CanExecute_OpenUrlCommand);
            SwitchTemplateSecretsCommand = new RelayCommand<TemplateType>(Execute_SwitchTemplateSecretsCommand, CanExecute_SwitchTemplateSecretsCommand);

            ShowEditMenuCommand = new RelayCommand<EncryptedSecretViewModel>(Execute_ShowEditMenuCommand, CanExecute_ShowEditMenuCommand);
            HideEditMenuCommand = new RelayCommand<ColumnDefinition>(Execute_HideEditMenuCommand, CanExecute_HideEditMenuCommand);
        }

        /*--Create--*/

        #region [CreateSecretCommand] - Добавление записи в систему

        public RelayCommandAsync? CreateSecretCommand { get; private set; }

        private async Task Execute_SaveSecretCommand()
        {
            var result = await _createSecretUseCase.Create(NameSecret!, UsernameSecret!, PasswordSecret!, EmailSecret!, SecretWorldSecret!, RecoveryKeysSecret!, UrlSecret, NotesSecret, false);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            SecretDatas.Add(new EncryptedSecretViewModel(result.Value!));
        }

        private bool CanExecute_SaveSecretCommand() 
            => !string.IsNullOrWhiteSpace(NameSecret) || !string.IsNullOrWhiteSpace(UsernameSecret) || !string.IsNullOrWhiteSpace(PasswordSecret) || !string.IsNullOrWhiteSpace(EmailSecret);

        #endregion

        /*--Delete--*/

        #region [DeleteSecretCommand] - Удаление записи из системы

        public RelayCommandAsync<EncryptedSecretViewModel>? DeleteSecretCommand { get; private set; }

        private async Task Execute_DeleteSecretCommand(EncryptedSecretViewModel secret)
        {
            var result = await _deleteSecretUseCase.DeleteAsync(secret.IdSecret);

            if (!result.IsSuccess)
            {
                var errors = string.Join(";", result.Errors);
                MessageBox.Show(errors);
                return;
            }

            SecretDatas.Remove(secret);
        }

        private bool CanExecute_DeleteSecretCommand(EncryptedSecretViewModel secret) => secret != null;

        #endregion

        /*--Update--*/

        #region [UpdateSecretCommand] - Обновление чувствительных данных

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
        }

        private bool CanExecute_UpdateFavoriteCommand(EncryptedSecretViewModel secret) => secret is not null;

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

            if (secret is null)
                SetNulEditFields();
            else
                SelectedSecretData = secret;

        } 

        private bool CanExecute_ShowEditMenuCommand(EncryptedSecretViewModel secret) => true;

        #endregion

        #region [HideEditMenuCommand] - Скрыть меню редактирование

        public RelayCommand<ColumnDefinition>? HideEditMenuCommand { get; private set; }

        private void Execute_HideEditMenuCommand(ColumnDefinition columnToReset)
        {
            SetVisibilityEditMenu(Visibility.Collapsed, 0);
            //columnToReset.Width = GridLength.Auto;
            EditMenuColumnWidth = new GridLength(0);
        } 

        private bool CanExecute_HideEditMenuCommand(ColumnDefinition columnToReset) 
            => VisibilityEditMenu != Visibility.Collapsed || VisibilityEditMenu != Visibility.Hidden;

        #endregion

        /*--Методы----------------------------------------------------------------------------------------*/

        #region Получение данных

        private async void GetSecrets()
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
                SecretDatas.Add(new EncryptedSecretViewModel(item));
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

    }
}
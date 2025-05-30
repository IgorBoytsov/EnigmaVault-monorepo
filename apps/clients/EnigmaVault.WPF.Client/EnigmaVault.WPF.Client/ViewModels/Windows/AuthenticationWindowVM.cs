using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.CountryCase;
using EnigmaVault.Application.UseCases.Abstractions.GanderCase;
using EnigmaVault.Domain.Results;
using EnigmaVault.WPF.Client.Command;
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.Services.Abstractions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace EnigmaVault.WPF.Client.ViewModels.Windows
{
    internal class AuthenticationWindowVM : BaseWindowViewModel
    {
        private readonly IWindowNavigationService _windowNavigationService;
        private readonly IAuthorizationService _authorizationService;

        private readonly IGetGendersUseCase _getGendersUseCase;
        private readonly IGetCountriesUseCase _getCountriesUseCase;

        public AuthenticationWindowVM(IWindowNavigationService windowNavigationService,
                                      IAuthorizationService authorizationService,
                                      IGetGendersUseCase getGendersUseCase,
                                      IGetCountriesUseCase getCountriesUseCase) : base(windowNavigationService)
        {
            _windowNavigationService = windowNavigationService;
            _authorizationService = authorizationService;

            _getGendersUseCase = getGendersUseCase;
            _getCountriesUseCase = getCountriesUseCase;

            SetValueCommands();
            CurrentAuthenticationType = AuthenticationType.Authentication;
        }

        /*--Коллекции-------------------------------------------------------------------------------------*/

        public ObservableCollection<GenderDto> Genders { get; set; } = [];

        public ObservableCollection<CountryDto> Countries { get; set; } = [];

        /*--Свойства--------------------------------------------------------------------------------------*/

        #region Текущий отображаемый контрол

        private AuthenticationType _currentAuthenticationType;
        public AuthenticationType CurrentAuthenticationType
        {
            get => _currentAuthenticationType;
            set
            {
                if (SetProperty(ref _currentAuthenticationType, value))
                {
                    SwitchOnAuthenticationControl?.RaiseCanExecuteChanged();
                    SwitchOnRegistrationControl?.RaiseCanExecuteChanged();
                    SwitchOnRecoveryAccessControl?.RaiseCanExecuteChanged();
                    OpenMainWindowCommand?.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion
        
        #region Вход

        private string? _authenticationLogin;
        public string? AuthenticationLogin
        {
            get => _authenticationLogin;
            set
            {
                SetProperty(ref _authenticationLogin, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _authenticationPassword;
        public string? AuthenticationPassword
        {
            get => _authenticationPassword;
            set 
            {
                SetProperty(ref _authenticationPassword, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion 

        #region Регистрация

        private string? _registrationLogin;
        public string? RegistrationLogin
        {
            get => _registrationLogin;
            set 
            {
                SetProperty(ref _registrationLogin, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            } 
        }

        private string? _registrationPassword;
        public string? RegistrationPassword
        {
            get => _registrationPassword;
            set
            {
                SetProperty(ref _registrationPassword, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _registrationUserName;
        public string? RegistrationUserName
        {
            get => _registrationUserName;
            set
            {
                SetProperty(ref _registrationUserName, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _registrationEmail;
        public string? RegistrationEmail
        {
            get => _registrationEmail;
            set 
            {
                SetProperty(ref _registrationEmail, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        } 
        
        private string? _registrationPhone;
        public string? RegistrationPhone
        {
            get => _registrationPhone;
            set
            {
                SetProperty(ref _registrationPhone, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }
        
        private GenderDto? _selectedRegistrationGender;
        public GenderDto? SelectedRegistrationGender
        {
            get => _selectedRegistrationGender;
            set
            {
                SetProperty(ref _selectedRegistrationGender, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }  
        
        private CountryDto? _selectedRegistrationCountry;
        public CountryDto? SelectedRegistrationCountry
        {
            get => _selectedRegistrationCountry;
            set
            {
                SetProperty(ref _selectedRegistrationCountry, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _registrationCodeVerification;
        public string? RegistrationCodeVerification
        {
            get => _registrationCodeVerification;
            set
            {
                SetProperty(ref _registrationCodeVerification, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Востановление доступа

        private string? _recoveryLogin;
        public string? RecoveryLogin
        {
            get => _recoveryLogin;
            set
            {
                SetProperty(ref _recoveryLogin, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }
        
        private string? _recoveryEmail;
        public string? RecoveryEmail
        {
            get => _recoveryEmail;
            set
            {
                SetProperty(ref _recoveryEmail, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _recoveryCodeVerification;
        public string? RecoveryCodeVerification
        {
            get => _recoveryCodeVerification;
            set
            {
                SetProperty(ref _recoveryCodeVerification, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        private string? _recoveryNewPassword;
        public string? RecoveryNewPassword
        {
            get => _recoveryNewPassword;
            set
            {
                SetProperty(ref _recoveryNewPassword, value);
                OpenMainWindowCommand?.RaiseCanExecuteChanged();
            }
        }

        #endregion

        /*--Команды---------------------------------------------------------------------------------------*/

        private void SetValueCommands()
        {
            SwitchOnAuthenticationControl = new RelayCommand<AuthenticationType>(Execute_SwitchOnAuthenticationControl, CanExecute_SwitchOnAuthenticationControl);
            SwitchOnRegistrationControl = new RelayCommand<AuthenticationType>(Execute_SwitchOnRegistrationControl, CanExecute_SwitchOnRegistrationControl);
            SwitchOnRecoveryAccessControl = new RelayCommand<AuthenticationType>(Execute_SwitchOnRecoveryAccessControl, CanExecute_SwitchOnRecoveryAccessControl);

            OpenMainWindowCommand = new RelayCommandAsync(Execute_OpenMainWindowAsync, CanExecute_OpenMainWindow);
        }

        #region Отображение контрола с авторизацией

        public RelayCommand<AuthenticationType>? SwitchOnAuthenticationControl { get; private set; }

        private void Execute_SwitchOnAuthenticationControl(AuthenticationType control) => CurrentAuthenticationType = control;

        private bool CanExecute_SwitchOnAuthenticationControl(AuthenticationType control) => CurrentAuthenticationType != AuthenticationType.Authentication;

        #endregion

        #region Отображение контрола с регистрацией

        public RelayCommand<AuthenticationType>? SwitchOnRegistrationControl { get; private set; }

        private async void Execute_SwitchOnRegistrationControl(AuthenticationType control) 
        {
            if (Genders.Count == 0)
                await GetGenders();

            if (Countries.Count == 0)
                await GetCountries();

            CurrentAuthenticationType = control;
        } 

        private bool CanExecute_SwitchOnRegistrationControl(AuthenticationType control) => CurrentAuthenticationType != AuthenticationType.Registration;

        #endregion

        #region Отображение контрола с востановлением доступа

        public RelayCommand<AuthenticationType>? SwitchOnRecoveryAccessControl { get; private set; }

        private void Execute_SwitchOnRecoveryAccessControl(AuthenticationType control) => CurrentAuthenticationType = control;

        private bool CanExecute_SwitchOnRecoveryAccessControl(AuthenticationType control) => CurrentAuthenticationType != AuthenticationType.RecoveryAccess;

        #endregion

        #region Выполнение аунтетификации/регастрацц/востановление доступа, с последующим открытием главного окна

        public RelayCommandAsync? OpenMainWindowCommand { get; private set; }

        private async Task Execute_OpenMainWindowAsync()
        {
            IsBusy = true;

            Func<Task> asyncActionToExecute = CurrentAuthenticationType switch
            {
                AuthenticationType.Authentication => async () => 
                {
                    Result<UserDto?> result = await _authorizationService.AuthenticationAsync(AuthenticationLogin!, AuthenticationPassword!);

                    if (result.IsSuccess && result.Value != null)
                    {
                        _windowNavigationService.Open(WindowName.MainWindow);
                        _windowNavigationService.TransmittingValue<UserDto>(WindowName.MainWindow, result.Value);
                        _windowNavigationService.Close(WindowName.AuthenticationWindow);
                    }
                    else
                        MessageBox.Show($"Ошибки аутентификации: {string.Join("; ", result.Errors)}");
                }
                ,
                AuthenticationType.Registration => async () =>
                {
                    Result<(string Login, string Password)?> result = await _authorizationService.RegistrationAsync(RegistrationLogin, RegistrationPassword, RegistrationUserName, RegistrationEmail, RegistrationPhone, SelectedRegistrationGender!.IdGender, SelectedRegistrationCountry!.IdCountry);
                    
                    if (result.IsSuccess && result.Value.HasValue)
                    {
                        AuthenticationLogin = result.Value.Value.Login;
                        AuthenticationPassword = result.Value.Value.Password;
                        CurrentAuthenticationType = AuthenticationType.Authentication; 
                        MessageBox.Show("Регистрация прошла успешно! Теперь вы можете войти, используя указанные данные.");
                    }
                    else
                        MessageBox.Show($"Ошибки регистрации: {string.Join("; ", result.Errors)}");
                }
                ,
                AuthenticationType.RecoveryAccess => async () =>
                {
                    Result<(string Login, string Password)?> result = await _authorizationService.RecoveryAccessAsync(RecoveryLogin, RecoveryEmail, RecoveryNewPassword);

                    if (result.IsSuccess && result.Value.HasValue)
                    {
                        AuthenticationLogin = result.Value.Value.Login;
                        AuthenticationPassword = result.Value.Value.Password;
                        CurrentAuthenticationType = AuthenticationType.Authentication; 
                        MessageBox.Show("Доступ восстановлен! Используйте новые данные для входа.");
                    }
                    else
                        MessageBox.Show($"Ошибки восстановления доступа: {string.Join("; ", result.Errors)}");
                }
                ,
                _ => () => 
                {
                    MessageBox.Show("Неизвестный тип операции. Действие не будет выполнено.");
                    return Task.CompletedTask;
                }
            };

            try
            {
                
                if (asyncActionToExecute != null)
                    await asyncActionToExecute();
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"Критическая ошибка при выполнении команды: {ex}");
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanExecute_OpenMainWindow()
        {
            return CurrentAuthenticationType switch
            {
                AuthenticationType.Authentication => !string.IsNullOrWhiteSpace(AuthenticationLogin) &&
                                                     !string.IsNullOrWhiteSpace(AuthenticationPassword),

                AuthenticationType.Registration => !string.IsNullOrWhiteSpace(RegistrationLogin) &&
                                                   !string.IsNullOrWhiteSpace(RegistrationPassword) &&
                                                   !string.IsNullOrWhiteSpace(RegistrationUserName) &&
                                                   !string.IsNullOrWhiteSpace(RegistrationEmail) &&
                                                   SelectedRegistrationGender != null &&
                                                   SelectedRegistrationCountry != null,

                AuthenticationType.RecoveryAccess => !string.IsNullOrWhiteSpace(RecoveryLogin) &&
                                                     !string.IsNullOrWhiteSpace(RecoveryEmail) &&
                                                     !string.IsNullOrWhiteSpace(RecoveryNewPassword),
                _ => false 
            };
        }

        #endregion

        /*--Методы----------------------------------------------------------------------------------------*/

        private async Task GetGenders()
        {
            Genders.Clear();

            var result = await _getGendersUseCase.GetGendersAsync();

            if (result.IsSuccess)
                foreach (var item in result.Value!)
                    Genders.Add(item);
            else
                MessageBox.Show(string.Join(";", result.Errors));
        }
        
        private async Task GetCountries()
        {
            Countries.Clear();

            var result = await _getCountriesUseCase.GetCountriesAsync();

            if (result.IsSuccess)
                foreach (var item in result.Value!)
                    Countries.Add(item);
            else
                MessageBox.Show(string.Join(";", result.Errors));
        }
    }
}
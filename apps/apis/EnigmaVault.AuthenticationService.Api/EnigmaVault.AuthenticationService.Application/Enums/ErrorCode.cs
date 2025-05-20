namespace EnigmaVault.AuthenticationService.Application.Enums
{
    public enum ErrorCode
    { 
        /// <summary>
        /// Выбрать при отсутствие надобности ошибки.
        /// </summary>
        None,

        // Общие ошибки

        /// <summary>
        /// Неизвестная ошибка. Использовать когда произошло что то не предвиденное.
        /// </summary>
        UnknownError,
        /// <summary>
        /// Ошибка валидации полей.
        /// </summary>
        ValidationFailed,
        /// <summary>
        /// Ошибка создания доменной модели.
        /// </summary>
        DomainCreationError,

        // Специфичные для регистрации

        /// <summary>
        /// Когда логин занят.
        /// </summary>
        LoginAlreadyTaken,
        /// <summary>
        /// Когда логина не существует.
        /// </summary>
        LoginNotExist,
        /// <summary>
        /// Когда почта уже используется в системе.
        /// </summary>
        EmailAlreadyRegistered, 
        /// <summary>
        /// Когда почта уже используется в системе.
        /// </summary>
        PhoneAlreadyRegistered,
        /// <summary>
        /// Пароль не соответствует минимальным требованием безопасности.
        /// </summary>
        WeakPassword,
        /// <summary>
        /// Если пароль не совпадает с нужным.
        /// </summary>
        InvalidPassword,
        /// <summary>
        /// Когда у пользователя указана не корректная роль.
        /// </summary>
        InvalidRole,
        /// <summary>
        /// Когда у пользователя указан не корректный статус аккаунта.
        /// </summary>
        InvalidAccountStatus,
        /// <summary>
        /// Ошибка сохранение пользователя в БД.
        /// </summary>
        SaveUserError,
    }
}
namespace EnigmaVault.WPF.Client.Command
{
    /// <summary>
    /// Обобщенная реализация ICommand для синхронных операций.
    /// </summary>
    /// <typeparam name="T">Тип параметра команды.</typeparam>
    public class RelayCommand<T> : CommandBase
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        /// <summary>
        /// Создает новую команду, которая всегда может быть выполнена.
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        /// <exception cref="ArgumentNullException">Если execute равен null.</exception>
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        /// <summary>
        /// Создает новую команду.
        /// </summary>
        /// <param name="execute">Логика выполнения.</param>
        /// <param name="canExecute">Логика проверки состояния выполнения.</param>
        /// <exception cref="ArgumentNullException">Если execute равен null.</exception>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            if (!TryConvertParameter<T>(parameter, out T typedParameter))
                if (typeof(T) != typeof(object) && parameter != null && !(parameter is T))
                    return false;

            return _canExecute == null || _canExecute(typedParameter);
        }

        public override void Execute(object parameter)
        {
            if (!TryConvertParameter<T>(parameter, out T typedParameter))
                if (typeof(T) != typeof(object) && parameter != null && !(parameter is T))
                    throw new ArgumentException($"Параметр имеет тип {parameter?.GetType().Name}, но ожидался совместимый с {typeof(T).Name}.", nameof(parameter));

            if (CanExecute(parameter))
                _execute(typedParameter);
        }
    }

    /// <summary>
    /// Необобщенная реализация ICommand для синхронных операций (использует object как параметр).
    /// </summary>
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(param => execute(), null)
            => ArgumentNullException.ThrowIfNull(execute);

        public RelayCommand(Action execute, Func<bool> canExecute) : base(param => execute(), param => canExecute())
            => ArgumentNullException.ThrowIfNull(execute);
    }
}
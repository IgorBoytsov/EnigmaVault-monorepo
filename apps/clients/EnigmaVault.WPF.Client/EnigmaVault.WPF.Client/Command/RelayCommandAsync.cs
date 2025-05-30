using System.Diagnostics;

namespace EnigmaVault.WPF.Client.Command
{
    /// <summary>
    /// Обобщенная реализация ICommand для асинхронных операций.
    /// </summary>
    /// <typeparam name="T">Тип параметра команды.</typeparam>
    public class RelayCommandAsync<T> : CommandBase
    {
        private readonly Func<T, Task> _executeAsync;
        private readonly Predicate<T> _canExecute;
        private volatile bool _isExecuting;

        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Создает новую асинхронную команду, которая всегда может быть выполнена (если не выполняется в данный момент).
        /// </summary>
        /// <param name="executeAsync">Асинхронная логика выполнения.</param>
        public RelayCommandAsync(Func<T, Task> executeAsync) : this(executeAsync, null) { }

        /// <summary>
        /// Создает новую асинхронную команду.
        /// </summary>
        /// <param name="executeAsync">Асинхронная логика выполнения.</param>
        /// <param name="canExecute">Логика проверки состояния выполнения.</param>
        public RelayCommandAsync(Func<T, Task> executeAsync, Predicate<T> canExecute)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            if (IsExecuting)
                return false;

            if (!TryConvertParameter<T>(parameter, out T typedParameter))
                if (typeof(T) != typeof(object) && parameter != null && !(parameter is T))
                    return false;

            return _canExecute == null || _canExecute(typedParameter);
        }

        /// <summary>
        /// Выполняет асинхронную логику команды.
        /// </summary>
        /// <param name="parameter">Параметр команды. Может быть null.</param>
        public override async void Execute(object parameter)
        {
            if (!TryConvertParameter<T>(parameter, out T typedParameter))
            {
                if (typeof(T) != typeof(object) && parameter != null && !(parameter is T))
                {
                    //TODO: залогировать ошибку.
                    Debug.WriteLine($"AsyncRelayCommand: Неверный тип параметра '{parameter?.GetType().Name}' для команды с типом параметра '{typeof(T).Name}'. Выполнение отменено.");
                    return;
                }
            }

            if (!CanExecute(parameter))
                return;

            IsExecuting = true;

            try
            {
                await _executeAsync(typedParameter);
            }
            catch (Exception ex) 
            {
                //TODO: Добавить логирование
                Debug.WriteLine($"Ошибка: {ex.Message}. Выполнение отменено.");
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }

    /// <summary>
    /// Необобщенная реализация ICommand для асинхронных операций.
    /// </summary>
    public class RelayCommandAsync : RelayCommandAsync<object>
    {
        public RelayCommandAsync(Func<Task> executeAsync) : base(async param => await executeAsync(), null) 
            => ArgumentNullException.ThrowIfNull(executeAsync);

        public RelayCommandAsync(Func<Task> executeAsync, Func<bool> canExecute) : base(async param => await executeAsync(), param => canExecute())
            => ArgumentNullException.ThrowIfNull(executeAsync);
    }
}
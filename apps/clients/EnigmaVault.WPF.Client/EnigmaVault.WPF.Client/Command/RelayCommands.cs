using System.Windows.Input;

namespace EnigmaVault.WPF.Client.Command
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        /// <summary>
        /// Конструктор для команды, которая всегда может быть выполнена.
        /// </summary>
        /// <param name="execute">Метод, выполняющий логику команды. Не должен быть null.</param>
        /// <exception cref="ArgumentNullException">Если execute равен null.</exception>
        public RelayCommand(Action<T> execute) : this(execute, null) { }

        /// <summary>
        /// Основной конструктор.
        /// </summary>
        /// <param name="execute">Метод, выполняющий логику команды. Не должен быть null.</param>
        /// <param name="canExecute">Метод, проверяющий, может ли команда быть выполнена. Может быть null.</param>
        /// <exception cref="ArgumentNullException">Если execute равен null.</exception>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            T typedParameter;
            if (parameter == null)
            {
                if (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                    return false;

                typedParameter = default(T); 
            }
            else if (parameter is not T)
            {
                try
                {
                    typedParameter = (T)Convert.ChangeType(parameter, typeof(T));
                }
                catch (Exception) 
                {
                    return false;
                }
            }
            else
            {
                typedParameter = (T)parameter;
            }

            return _canExecute(typedParameter);
        }

        public void Execute(object parameter)
        {
            T typedParameter;
            if (parameter == null)
            {
                if (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                {
                    if (typeof(T) == typeof(object))
                        typedParameter = (T)parameter;
                    else
                        typedParameter = (T)parameter; // Вызовет исключение.
                }
                else
                    typedParameter = default(T); // null для ссылочных типов и Nullable<TValue>
            }
            else if (!(parameter is T))
            {
                try
                {
                    typedParameter = (T)Convert.ChangeType(parameter, typeof(T));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Параметр имеет тип {parameter.GetType().Name} но не удалось преобразовать в ожидаемый тип {typeof(T).Name}.", nameof(parameter), ex);
                }
            }
            else
            {
                typedParameter = (T)parameter;
            }

            _execute(typedParameter);
        }

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute) : base(param => execute(), null) { }

        public RelayCommand(Action execute, Func<bool> canExecute) : base(param => execute(), param => canExecute()) { }
    }
}
using System.Windows.Input;

namespace EnigmaVault.WPF.Client.Command
{
    public abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        protected bool TryConvertParameter<T>(object parameter, out T typedParameter)
        {
            typedParameter = default;

            if (parameter == null)
            {
                if (typeof(T).IsValueType && Nullable.GetUnderlyingType(typeof(T)) == null)
                    return false;
                return true;
            }

            if (parameter is T directCast)
            {
                typedParameter = directCast;
                return true;
            }

            try
            {
                typedParameter = (T)Convert.ChangeType(parameter, typeof(T));
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }
    }
}
using EnigmaVault.WPF.Client.Enums;
using EnigmaVault.WPF.Client.ViewModels.Abstractions;

namespace EnigmaVault.WPF.Client.ViewModels.Windows
{
    internal class MainWindowVM : BaseWindowViewModel, IUpdatable
    {
        public MainWindowVM()
        {
            
        }

        void IUpdatable.Update<TData>(TData value, TransmittingParameter parameter)
        {
            
        }
    }
}
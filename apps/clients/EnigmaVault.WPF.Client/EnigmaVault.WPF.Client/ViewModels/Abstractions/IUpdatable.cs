using EnigmaVault.WPF.Client.Enums;

namespace EnigmaVault.WPF.Client.ViewModels.Abstractions
{
    internal interface IUpdatable
    {
        void Update<TData>(TData value, TransmittingParameter parameter = TransmittingParameter.None);
    }
}
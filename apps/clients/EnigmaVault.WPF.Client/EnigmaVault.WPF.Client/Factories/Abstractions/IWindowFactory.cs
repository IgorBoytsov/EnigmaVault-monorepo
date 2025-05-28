using System.Windows;

namespace EnigmaVault.WPF.Client.Factories.Abstractions
{
    internal interface IWindowFactory
    {
        //Window CreateWindow<TData>(TData? value, TransmittingParameter parameter = TransmittingParameter.None);
        Window CreateWindow();
    }
}
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Factories.Abstractions
{
    internal interface IPageFactory
    {
        //Page CreatePage<TData>(TData? value, TransmittingParameter parameter = TransmittingParameter.None);
        Page CreatePage();
    }
}
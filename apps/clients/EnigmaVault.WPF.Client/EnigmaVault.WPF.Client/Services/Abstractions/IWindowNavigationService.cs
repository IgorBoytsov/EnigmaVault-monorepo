using EnigmaVault.WPF.Client.Enums;

namespace EnigmaVault.WPF.Client.Services.Abstractions
{
    internal interface IWindowNavigationService
    {
        void Open(WindowName name, bool IsOpenDialog = false);
        void Close(WindowName windowName);
        void TransmittingValue<TData>(WindowName windowName, TData value, TransmittingParameter parameter = TransmittingParameter.None, bool isActive = false);

        void MinimizeWindow(WindowName windowName);
        void MaximizeWindow(WindowName windowName);
        void RestoreWindow(WindowName windowName);
    }
}
using EnigmaVault.WPF.Client.Enums;
using System.Windows.Controls;

namespace EnigmaVault.WPF.Client.Services.Abstractions
{
    internal interface IPageNavigationService
    {
        void Close(PageName pageName, FrameName frameName);
        void Navigate(PageName pageName, FrameName frameName);
        void RegisterFrame(FrameName frameName, Frame frame);
        void TransmittingValue<TData>(PageName pageName, FrameName frameName, TData value, TransmittingParameter typeParameter = TransmittingParameter.None, bool isNavigateAfterTransmitting = true, bool forceOpenPage = true);
        PageName GetCurrentDisplayedPage(FrameName frameName);
    }
}
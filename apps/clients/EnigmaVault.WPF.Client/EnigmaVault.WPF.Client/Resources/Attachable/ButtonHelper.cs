using System.Windows.Controls;
using System.Windows;

namespace EnigmaVault.WPF.Client.Resources.Attachable
{
    internal static class ButtonHelper
    {
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.RegisterAttached(
                "IconTemplate",
                typeof(ControlTemplate),
                typeof(ButtonHelper),
                new PropertyMetadata(null));

        public static void SetIconTemplate(DependencyObject element, ControlTemplate value)
        {
            element.SetValue(IconTemplateProperty, value);
        }

        public static ControlTemplate GetIconTemplate(DependencyObject element)
        {
            return (ControlTemplate)element.GetValue(IconTemplateProperty);
        }
    }
}
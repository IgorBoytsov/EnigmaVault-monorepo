using System.Globalization;
using System.Windows.Data;

namespace EnigmaVault.WPF.Client.Resources.ValueConverters
{
    internal class InvertedBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)value;
    }
}
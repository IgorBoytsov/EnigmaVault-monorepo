using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EnigmaVault.WPF.Client.Resources.ValueConverters
{
    class EqualityToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            object value1 = values[0];
            object value2 = values[1];

            if (value1 == DependencyProperty.UnsetValue || value2 == DependencyProperty.UnsetValue)
                return false;

            if (value1 == null)
                return false;

            return object.Equals(value1, value2);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
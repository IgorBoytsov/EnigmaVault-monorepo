using System.Globalization;
using System.Windows.Data;

namespace EnigmaVault.WPF.Client.Resources.ValueConverters
{
    internal class PopupCenterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] is not double targetWidth || values[1] is not double popupWidth)
                return 0.0;

            return (targetWidth - popupWidth) / 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using EnigmaVault.WPF.Client.Models.Display;
using System.Globalization;
using System.Windows.Data;

namespace EnigmaVault.WPF.Client.Resources.ValueConverters
{
    public class IdFolderToIsDeletableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id)
            {
                return id >= 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
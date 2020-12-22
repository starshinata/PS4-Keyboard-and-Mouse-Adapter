using System;
using System.Globalization;
using System.Windows.Data;
using PS4KeyboardAndMouseAdapter.Config;

namespace PS4KeyboardAndMouseAdapter.UI.Converters
{
    public class KeyNameToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return ((PhysicalKey)value).ToString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

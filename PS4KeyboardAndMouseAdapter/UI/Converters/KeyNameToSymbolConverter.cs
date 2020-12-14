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

            var keyName = ((PhysicalKey)value).ToString();

            if (keyName.Contains("Num"))
                keyName = keyName.Replace("Num", "");

            if (keyName.Equals("Left"))
                keyName = "\u21e6 Left";

            if (keyName.Equals("Up"))
                keyName = "\u21e7 Up";

            if (keyName.Equals("Right"))
                keyName = "\u21e8 Right";

            if (keyName.Equals("Down"))
                keyName = "\u21e9 Down";

            return keyName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

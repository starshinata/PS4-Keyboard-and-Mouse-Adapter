using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter.UI.Converters
{
    public class KeyNameToSymbolConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var keyName = ((Keyboard.Key) value).ToString();

            if (keyName.Contains("Num"))
                keyName = keyName.Replace("Num", "");

            if (keyName.Equals("Left"))
                keyName = "\u21e6";

            if (keyName.Equals("Up"))
                keyName = "\u21e7";

            if (keyName.Equals("Right"))
                keyName = "\u21e8";

            if (keyName.Equals("Down"))
                keyName = "\u21e9";

            return keyName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using Pizza.KeyboardAndMouseAdapter.UI;
using Serilog;
using System.Windows;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class ColourSchemeChanger
    {
        public static void Change(bool newValue)
        {
            Window window = Application.Current.MainWindow;
            if (window != null)
            {
                ((MainWindowView)window).ChangeColourScheme(newValue);
            }
            else
            {
                Log.Information("ColourSchemeChanger.Change() called but window is null");
            }
        }
    }
}

using AdonisUI;
using Pizza.KeyboardAndMouseAdapter.UI;
using System.Windows;

namespace PS4KeyboardAndMouseAdapter.Backend
{
    public class ColourSchemeChanger
    {
        public static void Change(bool newValue)
        {
            System.Uri colourScheme = newValue ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme;

            ResourceLocator.SetColorScheme(Application.Current.Resources, colourScheme);

            Window window = Application.Current.MainWindow;
            ((MainWindowView)window).ChangeScheme(colourScheme);
        }

    }
}

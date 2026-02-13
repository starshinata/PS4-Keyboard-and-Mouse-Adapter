using Pizza.KeyboardAndMouseAdapter.UI;
using Serilog;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    public class ColourSchemeChanger
    {
        public static void Change(bool newValue)
        {
            MainWindowView window = WindowUtil.GetMainWindowView();

            if (window != null)
            {
                window.ChangeColourScheme(newValue);
            }
            else
            {
                Log.Information("ColourSchemeChanger.Change() called but window is null");
            }
        }
    }
}

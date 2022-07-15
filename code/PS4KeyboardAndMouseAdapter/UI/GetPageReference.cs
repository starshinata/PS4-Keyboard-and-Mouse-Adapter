using Pizza.KeyboardAndMouseAdapter.UI.Pages;

namespace Pizza.KeyboardAndMouseAdapter.UI
{
    public class GetPageReference
    {
        public static AdvancedMappingsPage AdvancedMappings()
        {
            System.Windows.Window window = System.Windows.Application.Current.MainWindow;
            if (window != null)
            {
                MainWindowView mainWindowView = ((MainWindowView)window);
                return mainWindowView.getPageAdvancedMappings();
            }

            return null;
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using PS4KeyboardAndMouseAdapter.UI.Controls;
using Serilog;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class SimpleConfigPage : UserControl
    {
        public GamepadMappingController gamepadMappingController;

        public SimpleConfigPage()
        {
            Log.Information("SimpleConfigPage constructor START");
            InitializeComponent();
            gamepadMappingController = gamepadMappingControllerInner;
            Log.Information("SimpleConfigPage constructor END");
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }

    }
}

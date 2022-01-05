using Pizza.KeyboardAndMouseAdapter.UI.Controls;
using Serilog;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class SimpleConfigPage : UserControl
    {
        public GamepadMappingController gamepadMappingController;

        public SimpleConfigPage()
        {
            Log.Debug("SimpleConfigPage init IN");
            InitializeComponent();
            gamepadMappingController = gamepadMappingControllerInner;
            Log.Debug("SimpleConfigPage init OUT");
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }
    }
}

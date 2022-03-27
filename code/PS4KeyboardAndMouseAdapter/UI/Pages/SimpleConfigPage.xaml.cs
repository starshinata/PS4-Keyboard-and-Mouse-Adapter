using Pizza.KeyboardAndMouseAdapter.UI.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class SimpleConfigPage : UserControl
    {
        public GamepadMappingController gamepadMappingController;

        public SimpleConfigPage()
        {
            InitializeComponent();
            gamepadMappingController = gamepadMappingControllerInner;
        }

        public void ChangeScheme(System.Uri colourScheme)
        {
            gamepadMappingController.ChangeScheme(colourScheme);
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }
    }
}

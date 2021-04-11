using PS4KeyboardAndMouseAdapter.UI.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class SimpleConfigPage : UserControl
    {
        public GamepadMappingController gamepadMappingController;

        public SimpleConfigPage()
        {
            Console.WriteLine("SimpleConfigPage init IN");
            InitializeComponent();
            gamepadMappingController = gamepadMappingControllerInner;
            Console.WriteLine("SimpleConfigPage init OUT");
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }
    }
}

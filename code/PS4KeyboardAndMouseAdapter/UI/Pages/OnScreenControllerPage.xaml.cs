using Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class OnScreenControllerPage : UserControl
    {
        public GamepadMouseInput gamepadMouseInput;

        public OnScreenControllerPage()
        {
            InitializeComponent();
            gamepadMouseInput = gamepadMouseInputInner;
        }

        public void ChangeScheme(System.Uri colourScheme)
        {
            gamepadMouseInput.ChangeScheme(colourScheme);
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }
    }
}

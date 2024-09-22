using Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController;
using System;
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

        public void ChangeScheme(Uri colourScheme)
        {
            gamepadMouseInput.ChangeScheme(colourScheme);
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            WindowResized();

            ((MainViewModel)DataContext).RefreshData();
        }

        public void WindowResized()
        {
            stickLeft.RepaintStickCenter();
            stickRight.RepaintStickCenter();
        }
    }
}

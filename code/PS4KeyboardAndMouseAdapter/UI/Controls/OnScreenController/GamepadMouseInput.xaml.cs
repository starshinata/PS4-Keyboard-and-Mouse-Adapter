using AdonisUI;
using Serilog;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Button = System.Windows.Controls.Button;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController
{
    public partial class GamepadMouseInput : UserControl
    {
        private Button lastClickedButton;

        private readonly BitmapImage imageGamepadDark;
        private readonly BitmapImage imageGamepadLight;

        private readonly Uri uriMouseLeftDark;
        private readonly Uri uriMouseLeftLight;

        private readonly Uri uriMouseRightDark;
        private readonly Uri uriMouseRightLight;

        public GamepadMouseInput()
        {
            Log.Debug("GamepadMouseInput constructor IN");
            InitializeComponent();


            // imageGamepad
            imageGamepadDark = new BitmapImage();
            imageGamepadDark.BeginInit();
            imageGamepadDark.UriSource = new Uri("pack://application:,,,/images/ds4-dark-theme.png");
            imageGamepadDark.EndInit();

            imageGamepadLight = new BitmapImage();
            imageGamepadLight.BeginInit();
            imageGamepadLight.UriSource = new Uri("pack://application:,,,/images/ds4-light-theme.png");
            imageGamepadLight.EndInit();

            uriMouseLeftDark = new Uri("pack://application:,,,/images/mouse-left-button-dark-theme.svg");
            uriMouseLeftLight = new Uri("pack://application:,,,/images/mouse-left-button-light-theme.svg");
            uriMouseRightDark = new Uri("pack://application:,,,/images/mouse-right-button-dark-theme.svg");
            uriMouseRightLight = new Uri("pack://application:,,,/images/mouse-right-button-light-theme.svg");

            Log.Debug("GamepadMouseInput constructor OUT");
        }

        public void ChangeScheme(Uri colourScheme)
        {
            if (colourScheme == ResourceLocator.DarkColorScheme)
            {
                ChangeSchemeToDark();
            }
            else if (colourScheme == ResourceLocator.LightColorScheme)
            {
                ChangeSchemeToLight();
            }
        }

        public void ChangeSchemeToDark()
        {
            ImageGamepad.Source = imageGamepadDark;
            ImageMouseLeft.Source = uriMouseLeftDark;
            ImageMouseRight.Source = uriMouseRightDark;
        }

        public void ChangeSchemeToLight()
        {
            ImageGamepad.Source = imageGamepadLight;
            ImageMouseLeft.Source = uriMouseLeftLight;
            ImageMouseRight.Source = uriMouseRightLight;
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            //TODO
        }

    }
}

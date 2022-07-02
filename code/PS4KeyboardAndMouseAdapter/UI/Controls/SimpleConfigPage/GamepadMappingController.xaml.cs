using AdonisUI;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls
{
    public partial class GamepadMappingController : UserControl
    {
        private Button lastClickedButton;

        private readonly BitmapImage imageGamepadDark;
        private readonly BitmapImage imageGamepadLight;

        private readonly Uri uriMouseLeftDark;
        private readonly Uri uriMouseLeftLight;

        private readonly Uri uriMouseRightDark;
        private readonly Uri uriMouseRightLight;

        public GamepadMappingController()
        {
            Log.Debug("GamepadMappingController constructor IN");
            InitializeComponent();

            WaitingForKeyPress_Hide();


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

            Log.Debug("GamepadMappingController constructor OUT");
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
            WaitingForKeyPress_Show(button);
        }

        public void Handler_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (lastClickedButton != null && lastClickedButton.Tag != null)
            {
                foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>())
                {
                    if (Keyboard.IsKeyPressed(key))
                    {
                        if (key != Keyboard.Key.Escape)
                        {
                            VirtualKey vk = (VirtualKey)lastClickedButton.Tag;

                            PhysicalKey valueOld = UserSettings.GetInstance().KeyboardMappings[vk];

                            PhysicalKey valueNew = new PhysicalKey();
                            valueNew.KeyboardValue = key;
                            UserSettings.SetMapping(vk, valueOld, valueNew);
                        }

                        lastClickedButton = null;
                        ((MainViewModel)DataContext).RefreshData();
                        WaitingForKeyPress_Hide();
                    }
                }
            }
        }

        public void WaitingForKeyPress_Show(Button sender)
        {
            lastClickedButton = sender;

            WaitForKeyPress.Opacity = 0.7;
            ImageGamepad.Opacity = UIConstants.LOW_VISIBILITY;

            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                button.Opacity = UIConstants.LOW_VISIBILITY;
                button.IsEnabled = false;
            }
        }

        private void WaitingForKeyPress_Hide()
        {
            WaitForKeyPress.Opacity = 0;
            ImageGamepad.Opacity = 1;

            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                button.Opacity = 1;
                button.IsEnabled = true;
            }
        }
    }
}

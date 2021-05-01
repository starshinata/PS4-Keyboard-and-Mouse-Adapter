using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    public partial class GamepadMappingController : UserControl
    {
        private Button lastClickedButton;

        public GamepadMappingController()
        {
            Log.Information("GamepadMappingController constructor IN");
            InitializeComponent();

            WaitingForKeyPress_Hide();
            Log.Information("GamepadMappingController constructor OUT");
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
            JoystickImage.Opacity = UIConstants.LOW_VISIBILITY;

            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                button.Opacity = UIConstants.LOW_VISIBILITY;
                button.IsEnabled = false;
            }
        }

        private void WaitingForKeyPress_Hide()
        {
            WaitForKeyPress.Opacity = 0;
            JoystickImage.Opacity = 1;

            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                button.Opacity = 1;
                button.IsEnabled = true;
            }
        }
    }
}

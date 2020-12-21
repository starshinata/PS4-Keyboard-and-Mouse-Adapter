using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    /// <summary>
    /// Interaction logic for GamepadMappingController.xaml
    /// </summary>
    public partial class GamepadMappingController : UserControl
    {
        private const double LowOpacity = 0.1;

        private Button lastClickedButton;

        public UserSettings Settings;

        public Dictionary<VirtualKey, PhysicalKey> keyboardMappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();

        public GamepadMappingController()
        {
            Log.Information("GamepadMappingController constructor IN");
            Settings = UserSettings.GetInstance();
            InitializeComponent();

            InitializeButtons();

            WaitingForKeyPress_Hide();

            GetKeyboardMappings();
            Log.Information("GamepadMappingController constructor OUT");
        }

        private void GetKeyboardMappings() {
            if (Settings != null) {

                var virtualKeys = KeyUtility.GetVirtualKeyValues();
                Console.WriteLine("virtualKeys " + virtualKeys);
                foreach (VirtualKey key in virtualKeys)
                {
                    PhysicalKey pk = Settings.Mappings[key];
                    if (pk != null && pk.KeyboardValue!=Keyboard.Key.Unknown )
                    {
                        keyboardMappings[key] = Settings.Mappings[key];
                    }
                }
            }
        }

        private void InitializeButtons()
        {
            InitializeButton("buttonQQ");

        }

        private void InitializeButton(string buttonName)
        {
            Button button = FindName(buttonName) as Button;
            if (button != null)
            {
                VirtualKey virtualKey = (VirtualKey)button.Tag;

                button.Tag = virtualKey;
                Binding dataBinding = new Binding("Settings.Mappings[" + virtualKey + "]");
                button.SetBinding(ContentProperty, dataBinding);
            }
        }


        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
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
                            PhysicalKey pk = new PhysicalKey();
                            pk.KeyboardValue = key;
                            UserSettings.SetMapping((VirtualKey)lastClickedButton.Tag, pk);
                        }

                        lastClickedButton = null;
                        WaitingForKeyPress_Hide();

                        Settings = UserSettings.GetInstance();

                    }
                }
            }
        }

        public void WaitingForKeyPress_Show(Button sender)
        {

            lastClickedButton = sender;

            WaitForKeyPress.Opacity = 0.7;
            JoystickImage.Opacity = LowOpacity;

            foreach (var button in FindVisualChildren<Button>(this))
            {
                button.Opacity = LowOpacity;
                button.IsEnabled = false;
            }
        }

        private void WaitingForKeyPress_Hide()
        {
            WaitForKeyPress.Opacity = 0;
            JoystickImage.Opacity = 1;

            foreach (var button in FindVisualChildren<Button>(this))
            {
                button.Opacity = 1;
                button.IsEnabled = true;
            }
        }
    }
}

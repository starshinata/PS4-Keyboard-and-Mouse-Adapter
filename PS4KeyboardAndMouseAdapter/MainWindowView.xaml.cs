using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using PS4KeyboardAndMouseAdapter.UI;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView
    {
        public MainViewModel vm;

        private const double LowOpacity = 0.1;

        private Button lastClickedButton;

        public MainWindowView()
        {
            InitializeComponent();
            vm = (MainViewModel) DataContext;
            this.KeyDown += OnKeyDown;

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (lastClickedButton != null)
            {
                while (true)
                {
                    foreach (var key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>())
                    {
                        if (Keyboard.IsKeyPressed(key) && key != Keyboard.Key.P)
                        {
                            vm.SetMapping((VirtualKey) lastClickedButton.Tag, key);
                            lastClickedButton = null;
                            hidePressKey();
                            return;
                        }
                    }
                }
            }
        }

        private void displayPressKey()
        {
            PressKeyText.Opacity = 0.7;
            JoystickImage.Opacity = LowOpacity;

            foreach (var button in FindVisualChildren<Button>(this))
            {
                button.Opacity = LowOpacity;
                button.IsEnabled = false;
            }
        }

        private void hidePressKey()
        {
            PressKeyText.Opacity = 0;
            JoystickImage.Opacity = 1;

            foreach (var button in FindVisualChildren<Button>(this))
            {
                button.Opacity = 0.7;
                button.IsEnabled = true;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            lastClickedButton = (Button)sender;

            displayPressKey();

            var b = (Button) sender;
            //b.Content = vm.mappings[]
            //b.Content = ((string) b.Content) + new Random().Next(5);
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

        private void OnMouseHideToggleButtonClicked(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleButton) sender;

        }

        private void OnMouseSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            UITools.ShowWindow<MouseSettings>();
        }

        private void MainWindowView_OnActivated(object sender, EventArgs e)
        {
            vm.RemotePlayProcess.Refresh();
        }

        private void MainWindowView_OnDeactivated(object sender, EventArgs e)
        {
            vm.RemotePlayProcess.Refresh();
        }
    }
}

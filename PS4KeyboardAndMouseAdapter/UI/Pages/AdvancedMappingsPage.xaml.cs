using PS4KeyboardAndMouseAdapter.Config;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class AdvancedMappingsPage : UserControl
    {
        private Button lastClickedButton;
        private readonly double OpacityUnMappedButton = 0.5;
        private readonly UserSettings Settings;

        public AdvancedMappingsPage()
        {
            InitializeComponent();
            WaitingForKeyPress_Hide();

            Settings = UserSettings.GetInstance();
            PopulateWithMappings();
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
            RefreshButtonContents();
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            WaitingForKeyPress_Show(button);
        }

        private void Handler_AddMapping_GenericKeyDown(Keyboard.Key keyboardValue, MouseButton mouseValue)
        {

            if (lastClickedButton != null && lastClickedButton.Parent != null)
            {

                StackPanel parentStackPanel = (StackPanel)lastClickedButton.Parent;
                if (parentStackPanel.Tag != null)
                {
                    if (keyboardValue != Keyboard.Key.Escape)
                    {
                        VirtualKey vk = (VirtualKey)parentStackPanel.Tag;

                        int index = (int)lastClickedButton.Tag;
                        PhysicalKey valueOld = null;

                        if (Settings.MappingsContainsKey(vk))
                        {
                            if (index < Settings.Mappings[vk].PhysicalKeys.Count)
                            {
                                valueOld = Settings.Mappings[vk].PhysicalKeys[index];
                            }
                        }

                        PhysicalKey valueNew = new PhysicalKey();
                        valueNew.KeyboardValue = keyboardValue;
                        valueNew.MouseValue = mouseValue;

                        UserSettings.SetMapping(vk, valueOld, valueNew);
                    }

                    lastClickedButton = null;
                    ((MainViewModel)DataContext).RefreshData();
                    WaitingForKeyPress_Hide();
                }
            }
        }

        private void Handler_AddMapping_OnKeyboardKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Handler_AddMapping_OnKeyboardKeyDown");
            foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>())
            {
                if (Keyboard.IsKeyPressed(key))
                {
                    Handler_AddMapping_GenericKeyDown(key, MouseButton.Unknown);
                }
            }
        }

        private void Handler_AddMapping_OnMouseDown(object sender, RoutedEventArgs e)
        {
            Array mouseButtons = Enum.GetValues(typeof(Mouse.Button));
            foreach (Mouse.Button button in mouseButtons)
            {
                if (Mouse.IsButtonPressed(button))
                {
                    MouseButton mouseButton = (MouseButton)button;
                    Handler_AddMapping_GenericKeyDown(Keyboard.Key.Unknown, mouseButton);
                }
            }
        }

        private void Handler_AddMapping_OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            Handler_AddMapping_GenericKeyDown(Keyboard.Key.Unknown, MouseButton.Left);
        }

        private void Handler_AddMapping_OnMouseScroll(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Handler_AddMapping_OnMouseScroll");
            Console.WriteLine(DateTime.Now);


            Console.WriteLine("e" + e);
            Console.WriteLine("e" + e.GetType());

            System.Windows.Input.MouseWheelEventArgs mw = (System.Windows.Input.MouseWheelEventArgs)e;
            Console.WriteLine("mw" + mw);
            Console.WriteLine("Delta" + mw.Delta);
            Console.WriteLine("device" + mw.Device);
            Console.WriteLine("mw.Handled" + mw.Handled);
            Console.WriteLine("mw.MouseDevice" + mw.MouseDevice);
            //Console.WriteLine("X" + mw.X);
            //Console.WriteLine("Y" + mw.Y);//
            //Console.WriteLine("Y" + mw.Wheel);

            //        MouseButton mouseButton = (MouseButton)button;
            //Handler_AddMapping_GenericKeyDown(Keyboard.Key.Unknown, mouseButton);
        }

        private void PopulateWithMappings()
        {
            Thickness buttonMargin = new Thickness();
            buttonMargin.Left = 15;

            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey vk in virtualKeys)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Tag = vk;

                TextBlock textblock = new TextBlock()
                {
                    FontWeight = FontWeights.Bold
                };
                textblock.Text = vk.ToString();
                textblock.Width = 100;
                stackPanel.Children.Add(textblock);

                for (int i = 0; i < Settings.AdvancedMappingPage_MappingsToShow; i++)
                {

                    Button button = new Button();
                    button.Click += Handler_ButtonClicked;
                    button.Margin = buttonMargin;
                    button.Tag = i;
                    button.Width = 120;

                    stackPanel.Children.Add(button);
                }

                mappingHolder.Children.Add(stackPanel);
            }

            RefreshButtonContents();
        }

        public void RefreshButtonContents()
        {
            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                // assume unmapped first
                button.Content = "set mapping";
                button.Opacity = OpacityUnMappedButton;

                if (button != null && button.Tag != null)
                {
                    StackPanel parentStackPanel = (StackPanel)button.Parent;
                    if (parentStackPanel != null && parentStackPanel.Tag != null)
                    {
                        VirtualKey vk = (VirtualKey)parentStackPanel.Tag;
                        if (Settings.Mappings != null && Settings.MappingsContainsKey(vk))
                        {
                            PhysicalKeyGroup pkg = Settings.Mappings[vk];
                            if (pkg.PhysicalKeys != null)
                            {

                                int index = (int)button.Tag;
                                if (index < pkg.PhysicalKeys.Count)
                                {
                                    PhysicalKey pk = pkg.PhysicalKeys[index];
                                    if (pk != null)
                                    {
                                        button.Content = pk.ToString();
                                        button.Opacity = 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Needs to public
        public void WaitingForKeyPress_Show(Button sender)
        {
            lastClickedButton = sender;

            scrollViewer.Visibility = Visibility.Hidden;
            // if mappingHolder is enabled then buttons are still clickable
            // EVEN if you set button.IsEnabled = false
            mappingHolder.IsEnabled = false;

            /*
            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                button.Opacity = UIConstants.LowVisibility;
            }

            foreach (TextBlock textBlock in UITools.FindVisualChildren<TextBlock>(this))
            {
                textBlock.Opacity = UIConstants.LowVisibility;
            }
            */
            WaitForKeyPress_1.Visibility = Visibility.Visible;
            /*
            WaitForKeyPress_1.Opacity = 1;
            WaitForKeyPress_2.Opacity = 1;
            WaitForKeyPress_3.Opacity = 1;
            WaitForKeyPress_4.Opacity = 1;
            WaitForKeyPress_5.Opacity = 1;
            */

            Console.WriteLine("WaitingForKeyPress_Show FOCUS todo");
            WaitForKeyPress_2.Focus();
            Console.WriteLine("WaitingForKeyPress_Show FOCUSSE ");
            
        }

        private void WaitingForKeyPress_Hide()
        {
            /*
            WaitForKeyPress_1.Opacity = 0;
            WaitForKeyPress_2.Opacity = 0;
            WaitForKeyPress_3.Opacity = 0;
            WaitForKeyPress_4.Opacity = 0;
            WaitForKeyPress_5.Opacity = 0;
            */
                WaitForKeyPress_1.Visibility = Visibility.Hidden;

            mappingHolder.IsEnabled = true;
            scrollViewer.Visibility = Visibility.Visible;
            RefreshButtonContents();
            foreach (TextBlock textBlock in UITools.FindVisualChildren<TextBlock>(this))
            {
                textBlock.Opacity = 1;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using PS4KeyboardAndMouseAdapter.Config;
using Button = System.Windows.Controls.Button;
using Keyboard = SFML.Window.Keyboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class AdvancedMappingsPage : UserControl
    {
        private Button lastClickedButton;
        private double OpacityUnMappedButton = 0.5;
        private readonly UserSettings Settings;

        public AdvancedMappingsPage()
        {
            InitializeComponent();
            WaitForKeyPress_1.Opacity = 0;

            Settings = UserSettings.GetInstance();
            PopulateWithMappings();
        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
            Console.WriteLine("GotFocusLocal");
            Console.WriteLine("REMINDER - do refresh logic for when you load profiles");
            Console.WriteLine("REMINDER - do mouse click binding");
            Console.WriteLine("REMINDER - delte bind button");
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Handler_ButtonClicked");
            Button button = (Button)sender;
            WaitingForKeyPress_Show(button);
        }

        public void Handler_OnKeyboardKeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("Handler_OnKeyboardKeyDown");
            Console.WriteLine("lastClickedButton " + lastClickedButton);

            if (lastClickedButton != null && lastClickedButton.Parent != null)
            {
                StackPanel parentStackPanel = (StackPanel)lastClickedButton.Parent;
                if (parentStackPanel.Tag != null)
                {
                    Console.WriteLine("parentStackPanel.Tag " + parentStackPanel.Tag);

                    foreach (Keyboard.Key key in Enum.GetValues(typeof(Keyboard.Key)).Cast<Keyboard.Key>())
                    {
                        if (Keyboard.IsKeyPressed(key))
                        {

                            if (key != Keyboard.Key.Escape)
                            {
                                VirtualKey vk = (VirtualKey)parentStackPanel.Tag;

                                PhysicalKey valueOld = (PhysicalKey)lastClickedButton.Tag;

                                PhysicalKey valueNew = new PhysicalKey();
                                valueNew.KeyboardValue = key;

                                lastClickedButton.Tag = valueNew;

                                UserSettings.SetMapping(vk, valueOld, valueNew);

                                lastClickedButton.Content = valueNew.ToString();
                            }

                            lastClickedButton = null;
                            ((MainViewModel)DataContext).RefreshData();
                            WaitingForKeyPress_Hide();
                        }
                    }

                }
            }
        }

        private void PopulateWithMappings()
        {
            Thickness buttonMargin = new Thickness();
            buttonMargin.Left = 15;

            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey vk in virtualKeys)
            {
                Console.WriteLine("PopulateWithMappings() vk " + vk);
                
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Tag = vk;

                TextBlock textblock = new TextBlock() {
                    FontWeight = FontWeights.Bold
                };
                textblock.Text = vk.ToString();
                textblock.Width = 100;
                stackPanel.Children.Add(textblock);

                Console.WriteLine("PopulateWithMappings() vk " + vk);
                Console.WriteLine("PopulateWithMappings() Settings.Mappings.Count " + Settings.Mappings.Count());

                if (Settings.Mappings.ContainsKey(vk))
                {
                    PhysicalKeyGroup pkg = Settings.Mappings[vk];
                    if (pkg != null && pkg.PhysicalKeys != null)
                    {

                        for (int i = 0; i < Settings.AdvancedMappingPage_MappingsToShow; i++)
                        {

                            Button button = new Button();
                            button.Click += Handler_ButtonClicked;
                            button.Margin = buttonMargin;
                            button.Width = 100;

                            if (i < pkg.PhysicalKeys.Count)
                            {
                                PhysicalKey pk = pkg.PhysicalKeys[i];
                                button.Content = pk.ToString();
                                button.Tag = pk;
                            }
                            else
                            {
                                button.Content = "set mapping";
                                button.Opacity = OpacityUnMappedButton;
                            }

                            stackPanel.Children.Add(button);
                        }
                    }
                }

                mappingHolder.Children.Add(stackPanel);
            }
        }

        public void WaitingForKeyPress_Show(Button sender)
        {
            Console.WriteLine("WaitingForKeyPress_Show");
            lastClickedButton = sender;

            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                button.Opacity = UIConstants.LowVisibility;
                button.IsEnabled = false;
            }

            foreach (TextBlock textBlock in UITools.FindVisualChildren<TextBlock>(this))
            {
                textBlock.Opacity = UIConstants.LowVisibility;
            }

            WaitForKeyPress_1.Opacity = 1;
            WaitForKeyPress_2.Opacity = 1;
            WaitForKeyPress_3.Opacity = 1;
            WaitForKeyPress_4.Opacity = 1;
        }


        private void WaitingForKeyPress_Hide()
        {
            Console.WriteLine("WaitingForKeyPress_Hide");
            WaitForKeyPress_1.Opacity = 0;
            WaitForKeyPress_2.Opacity = 0;
            WaitForKeyPress_3.Opacity = 0;
            WaitForKeyPress_4.Opacity = 0;


            foreach (Button button in UITools.FindVisualChildren<Button>(this))
            {
                if (button.Tag != null)
                {
                    button.Opacity = 1;
                }
                else
                {
                    button.Opacity = OpacityUnMappedButton;
                }

                button.IsEnabled = true;
            }
            foreach (TextBlock textBlock in UITools.FindVisualChildren<TextBlock>(this))
            {
                textBlock.Opacity = 1;
            }

        }
    }
}

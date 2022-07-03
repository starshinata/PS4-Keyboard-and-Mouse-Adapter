using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class AdvancedMappingsPage : UserControl
    {
        private readonly double OpacityUnMappedButton = 0.5;
        private readonly UserSettingsV2 Settings;

        public AdvancedMappingsPage()
        {
            Log.Debug("AdvancedMappingsPage init IN");
            InitializeComponent();
            EditMapping_Hide();

            Settings = UserSettingsContainer.GetInstance();
            PopulateWithMappings();
            Log.Debug("AdvancedMappingsPage init OUT");
        }

        // Needs to public
        public void EditMapping_Show(Button button)
        {
            // TODO Send something to EditMappingControl


            scrollViewer.Visibility = Visibility.Hidden;

            // if mappingHolder is enabled then buttons are still clickable
            // EVEN if you set button.IsEnabled = false
            mappingHolder.IsEnabled = false;

            //TODO
            //WaitForKeyPress_1.Visibility = Visibility.Visible;

            // WaitForKeyPress_2 is the stack panel
            // if we dont focus it then keyboard key presses might not register

            //TODO
            //WaitForKeyPress_2.Focus();

            editMappingControl.Visibility = Visibility.Visible;
            mappingHolder.Visibility = Visibility.Collapsed;
        }

        public void EditMapping_Hide()
        {
            //TODO
            //WaitForKeyPress_1.Visibility = Visibility.Hidden;
            mappingHolder.IsEnabled = true;
            editMappingControl.Visibility = Visibility.Collapsed;
            mappingHolder.Visibility = Visibility.Visible;

            RefreshButtonContents();
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            EditMapping_Show(button);
        }

        private void Handler_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("AdvancedMappingsPage.LOADED");
            RefreshButtonContents();
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
            Log.Debug("AdvancedMappingsPage.RefreshButtonContents IN");
            Log.Debug("AdvancedMappingsPage.RefreshButtonContents Button Count " + UITools.FindVisualChildren<Button>(this).Count());
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
            Log.Debug("AdvancedMappingsPage.RefreshButtonContents OUT");
        }

    }
}

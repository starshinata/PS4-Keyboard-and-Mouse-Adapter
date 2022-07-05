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
        private readonly UserSettingsV3 Settings;

        private Thickness buttonMargin;


        private int MaxColumnCount = 0;
        private Dictionary<string, List<Mapping>> CompositeKeyMappings = new Dictionary<string, List<Mapping>>();

        public AdvancedMappingsPage()
        {
            Log.Debug("AdvancedMappingsPage init IN");
            InitializeComponent();

            buttonMargin = new Thickness();
            buttonMargin.Left = 15;

            Settings = UserSettingsContainer.GetInstance(); 
            EditMapping_Hide();

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

            PopulateWithMappings();
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            EditMapping_Show(button);
        }

        private void Handler_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("AdvancedMappingsPage.LOADED");
            PopulateWithMappings();
        }

        private void Refresh_Internal_CompositeKeyMappings()
        {
            int _MaxColumnCount = 0;

            Dictionary<string, List<Mapping>> _VirtualKeyMappings = new Dictionary<string, List<Mapping>>();

            foreach (Mapping mapping in Settings.Mappings)
            {
                string key = mapping.GetCompositeKeyVirtual();

                if (!_VirtualKeyMappings.ContainsKey(key))
                {
                    _VirtualKeyMappings.Add(key, new List<Mapping>());
                }

                _VirtualKeyMappings[key].Add(mapping);
                if (_VirtualKeyMappings[key].Count > MaxColumnCount)
                {
                    _MaxColumnCount = _VirtualKeyMappings[key].Count;
                }

            }

            MaxColumnCount = _MaxColumnCount;

            CompositeKeyMappings = _VirtualKeyMappings;
        }

        public void PopulateWithMappings()
        {

            //TODO delete all buttons

            Refresh_Internal_CompositeKeyMappings();

            foreach (string vk in CompositeKeyMappings.Keys)
            {
                PopulateWithMappings_CompositKey(vk);
            }

        }

        private void PopulateWithMappings_CompositKey(string compositeKey)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Tag = compositeKey;

            TextBlock textblock = new TextBlock()
            {
                FontWeight = FontWeights.Bold
            };
            textblock.Text = compositeKey;
            textblock.Width = 100;
            stackPanel.Children.Add(textblock);

            for (int i = 0; i < MaxColumnCount; i++)
            {
                Mapping mapping = CompositeKeyMappings[compositeKey].ElementAt(i);
                Button button = PopulateWithMappings_GetButton(mapping);
                stackPanel.Children.Add(button);
            }

            mappingHolder.Children.Add(stackPanel);
        }

        private Button PopulateWithMappings_GetButton(Mapping mapping)
        {
            Button button = new Button();
            button.Click += Handler_ButtonClicked;
            button.Margin = buttonMargin;

            button.Width = 120;

            if (mapping == null)
            {
                button.Content = "set mapping";
                button.Opacity = OpacityUnMappedButton;
            }
            else
            {
                button.Content = mapping.GetCompositeKeyPhysical();
                button.Opacity = 1;
            }

            return button;
        }
      
    }
}

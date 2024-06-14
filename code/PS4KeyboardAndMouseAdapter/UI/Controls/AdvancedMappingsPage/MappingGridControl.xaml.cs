using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.AdvancedMappingsPage
{
    public partial class MappingGridControl : UserControl
    {
        private readonly double OpacityUnMappedButton = 0.5;
        private readonly UserSettingsV3 UserSettings;

        private Thickness buttonMargin;

        private int MaxColumnCount = 0;

        public MappingGridControl()
        {
            InitializeComponent();

            buttonMargin = new Thickness();
            buttonMargin.Left = 15;

            UserSettings = UserSettingsContainer.GetInstance();
        }

        private void Handler_ButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            var page = GetPageReference.AdvancedMappings();
            if (page != null)
            {
                page.EditMapping_Show(button);
            }
        }

        private void Handler_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Debug("AdvancedMappingsPage.LOADED");
            PopulateWithMappings();
        }

        public void PopulateWithMappings()
        {
            PopulateWithMappings_DeleteExistingButtons();

            PopulateWithMappings_GetColumnCount();

            foreach (string vk in UserSettings.Mappings_ForAdvancedMappingsPage.Keys)
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
                Mapping mapping = null;
                if (i < UserSettings.Mappings_ForAdvancedMappingsPage[compositeKey].Count)
                {
                    mapping = UserSettings.Mappings_ForAdvancedMappingsPage[compositeKey].ElementAt(i);
                }

                Button button = PopulateWithMappings_GetButton(mapping);
                stackPanel.Children.Add(button);
            }

            mappingHolder.Children.Add(stackPanel);
        }

        private void PopulateWithMappings_DeleteExistingButtons()
        {
            mappingHolder.Children.Clear();
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
                button.Tag = mapping;
            }

            return button;
        }

        private void PopulateWithMappings_GetColumnCount()
        {
            int _MaxColumnCount = 0;

            foreach (List<Mapping> mappings in UserSettings.Mappings_ForAdvancedMappingsPage.Values)
            {
                if (mappings.Count > _MaxColumnCount)
                {
                    _MaxColumnCount = mappings.Count;
                }
            }

            MaxColumnCount = _MaxColumnCount + 1;
        }

    }
}

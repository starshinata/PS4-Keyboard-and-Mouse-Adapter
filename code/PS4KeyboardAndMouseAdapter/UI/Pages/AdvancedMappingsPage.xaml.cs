using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;

namespace Pizza.KeyboardAndMouseAdapter.UI.Pages
{
    public partial class AdvancedMappingsPage : UserControl
    {
        public AdvancedMappingsPage()
        {
            InitializeComponent();

            EditMapping_Hide();
        }

        public void EditMapping_Show(Button button)
        {
            // if mappingHolder is enabled then buttons are still clickable
            // EVEN if you set button.IsEnabled = false
            mappingGrid.IsEnabled = false;
            mappingGrid.Visibility = Visibility.Collapsed;

            Mapping mapping = null;
            if (button.Tag != null)
            {
                mapping = (Mapping)button.Tag;
            }
            editMappingControl.ShowThis(mapping);

            stackpanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackpanel.VerticalAlignment = VerticalAlignment.Center;

            editMappingControl.Visibility = Visibility.Visible;
            editMappingControl.Focus();
        }

        public void EditMapping_Hide()
        {
            mappingGrid.IsEnabled = true;
            editMappingControl.Visibility = Visibility.Collapsed;
            mappingGrid.Visibility = Visibility.Visible;

            //
            stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            stackpanel.VerticalAlignment = VerticalAlignment.Top;

            mappingGrid.PopulateWithMappings();
        }

        public void Refresh()
        {
            mappingGrid.PopulateWithMappings();
        }
    }
}

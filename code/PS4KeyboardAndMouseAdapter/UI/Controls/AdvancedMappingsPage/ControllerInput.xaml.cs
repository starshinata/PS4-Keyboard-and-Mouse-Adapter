using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.AdvancedMappingsPage
{
    public partial class ControllerInput : UserControl
    {

        public ControllerInput()
        {
            InitializeComponent();
            combobox.ItemsSource = KeyUtility.GetVirtualKeyValues();
        }

        private void Button_RemoveThis(object sender, System.Windows.RoutedEventArgs e)
        {
            Panel panel = (Panel)this.Parent;
            panel.Children.Remove(this);
        }

        public VirtualKey GetSelected()
        {
            return (VirtualKey)combobox.SelectedItem;
        }

        public void SetSelected(VirtualKey vk)
        {
            combobox.SelectedItem = vk;
        }

    }
}


using System.Windows;
using System.Windows.Controls;
using PS4KeyboardAndMouseAdapter.Config;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class AdvancedMappingsPage : UserControl
    {
        UserSettings Settings;

        public AdvancedMappingsPage()
        {
            InitializeComponent();

            Settings = UserSettings.GetInstance();

            AddStuff();
        }

        private void AddStuff()
        {
            var virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey vk in virtualKeys)
            {

                TextBlock textblock = new TextBlock() {
                    Text = string.Format("{0}", vk),
                };
                mappingHolder.Children.Add(textblock);

                PhysicalKeyGroup pkg = Settings.Mappings[vk];
                if (pkg != null && pkg.PhysicalKeys != null)
                {

                    foreach (PhysicalKey pk in pkg.PhysicalKeys)
                    {
                        Button button = new Button();
                        button.Content = string.Format("value '{0}'", pk);
                        mappingHolder.Children.Add(button);
                    }
                }

            }

        }

        private void GotFocusLocal(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).RefreshData();
        }

    }
}

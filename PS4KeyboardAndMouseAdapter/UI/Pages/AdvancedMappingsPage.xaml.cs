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

            addStuff();
        }

        private void addStuff() {
            var virtualKeys = System.Enum.GetValues(typeof(VirtualKey));
            foreach (VirtualKey vk in virtualKeys)
            {
                if (vk != VirtualKey.NULL)
                {
                    TextBlock textblock = new TextBlock() {
                        Text = string.Format("{0}", vk),

                    };
                    Button button = new Button();


                    if (Settings.Mappings[vk].KeyboardValue != SFML.Window.Keyboard.Key.Unknown)
                    {
                        button.Content = string.Format("value '{0}'", Settings.Mappings[vk]);
                    }

                    mappingHolder.Children.Add(textblock);
                    mappingHolder.Children.Add(button);
                }

            }
        }
    }
}

using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    /// <summary>
    /// Interaction logic for MouseSettingsControl.xaml
    /// </summary>
    public partial class MouseSettingsControl : UserControl
    {
        private UserSettings Settings;

        public MouseSettingsControl()
        {
            InitializeComponent();
            Settings = UserSettings.GetInstance();
        }
    }
}

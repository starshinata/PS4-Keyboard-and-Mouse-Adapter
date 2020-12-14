using PS4KeyboardAndMouseAdapter.Config;
using System.Windows.Controls;


namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    public partial class MouseSettingsSimpleControl : UserControl
    {
        private UserSettings Settings;

        public MouseSettingsSimpleControl()
        {
            InitializeComponent();
            Settings = UserSettings.GetInstance();
        }
    }
}

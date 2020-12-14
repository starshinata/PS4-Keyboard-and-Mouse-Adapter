using PS4KeyboardAndMouseAdapter.Config;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
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

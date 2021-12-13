using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    public partial class MouseSettingsSimpleControl : UserControl
    {
        public MouseSettingsSimpleControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Log.Information("MouseSettingsSimpleControl.sendX");
            Log.Information("InstanceSettings.GetInstance().GetVigemInjector()");
            Log.Information(InstanceSettings.GetInstance().GetVigemInjector().ToString());
            InstanceSettings.GetInstance().GetVigemInjector().sendX();
        }
    }
}

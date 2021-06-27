using System;
using System.Windows.Controls;
using PS4KeyboardAndMouseAdapter.Config;
using PS4RemotePlayInjection;
using Serilog;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    public partial class MiscellaneousSettingsControl : UserControl
    {

        public MiscellaneousSettingsControl()
        {
            InitializeComponent();

            sliderVolume.ValueChanged += Handler_RemotePlayVolumeChange;
        }

        private void Handler_RemotePlayVolumeChange(object sender, EventArgs e)
        {
            if (InstanceSettings.GetInstance().RemotePlayProcess != null)
            {
                AudioManager.SetApplicationVolume(InstanceSettings.GetInstance().RemotePlayProcess.Id, UserSettings.GetInstance().RemotePlayVolume);
            }
        }

        private void Handler_ResetRemotePlay(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowUtil.ResetWindowLocation(RemotePlayConstants.WINDOW_NAME);
        }

        private void Handler_Ps4ToolBarVisibleToggle(object sender, System.Windows.RoutedEventArgs e)
        {
            Log.Debug("MiscellaneousSettingsControl.Handler_Ps4ToolBarVisibleToggle");
            Utility.IsToolBarVisible = !Utility.IsToolBarVisible ;
        }
    }
}

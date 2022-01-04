using PS4KeyboardAndMouseAdapter.Config;
using System;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.MiscellaneousSettings
{
    public partial class MiscellaneousSettingsGeneralControl : UserControl
    {

        public MiscellaneousSettingsGeneralControl()
        {
            InitializeComponent();

            sliderVolume.ValueChanged += Handler_RemotePlayVolumeChange;
        }

        private void Handler_RemotePlayVolumeChange(object sender, EventArgs e)
        {
            if (InstanceSettings.GetInstance().GetRemotePlayProcess() != null)
            {
                AudioManager.SetApplicationVolume(InstanceSettings.GetInstance().GetRemotePlayProcess().Id, UserSettings.GetInstance().RemotePlayVolume);
            }
        }

        private void Handler_ResetRemotePlay(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowUtil.ResetWindowLocation(RemotePlayConstants.WINDOW_NAME);
        }

    }
}

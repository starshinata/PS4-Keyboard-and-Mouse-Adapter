using System;
using System.Windows.Controls;
using PS4KeyboardAndMouseAdapter.Config;

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
            MainViewModel mvm = (MainViewModel)DataContext;
            if (mvm != null &&
                mvm.RemotePlayInjector != null &&
                mvm.RemotePlayInjector.RemotePlayProcess != null)
            {
                AudioManager.SetApplicationVolume(mvm.RemotePlayInjector.RemotePlayProcess.Id, UserSettings.GetInstance().RemotePlayVolume);

            }
        }

        private void Handler_ResetRemotePlay(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowUtil.ResetWindowLocation(RemotePlayConstants.WINDOW_NAME);
        }

    }
}

using System;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{
    public partial class MiscellaneousSettingsPage : UserControl
    {

        public MiscellaneousSettingsPage()
        {
            InitializeComponent();

            sliderVolume.ValueChanged += Handler_RemotePlayVolumeChange;
        }

        private void Handler_RemotePlayVolumeChange(object sender, EventArgs e)
        {
            Console.WriteLine("volume change");
        }

        private void Handler_ResetRemotePlay(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowUtil.ResetWindowLocation(RemotePlayConstants.WINDOW_NAME);
        }

    }
}

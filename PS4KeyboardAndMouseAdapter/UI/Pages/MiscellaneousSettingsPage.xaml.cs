using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Pages
{

    public partial class MiscellaneousSettingsPage : UserControl
    {

        public MiscellaneousSettingsPage()
        {
            InitializeComponent();
        }

        private void Handler_ResetRemotePlay(object sender, System.Windows.RoutedEventArgs e)
        {
            WindowUtil.ResetWindowLocation(RemotePlayConstants.WINDOW_NAME);
        }
    }
}

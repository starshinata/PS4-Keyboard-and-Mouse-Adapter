using Pizza.KeyboardAndMouseAdapter.Backend;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class RemotePlayLaunchedControl : UserControl
    {
        public RemotePlayLaunchedControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindowView window = WindowUtil.GetMainWindowView();
            window.WelcomeStep2Done_RemotePlayStarted();
        }
    }
}

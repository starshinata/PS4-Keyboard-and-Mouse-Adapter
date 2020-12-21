using System;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter
{

    public partial class MainWindowView
    {

        public MainViewModel vm;

        public MainWindowView()
        {
            InitializeComponent();
            vm = (MainViewModel)DataContext;
            KeyDown += MainWindowView_OnKeyDown;
        }

        private void MainWindowView_OnActivated(object sender, EventArgs e)
        {
            vm.RemotePlayProcess.Refresh();
        }

        private void MainWindowView_OnDeactivated(object sender, EventArgs e)
        {
            vm.RemotePlayProcess.Refresh();
        }

        public void MainWindowView_OnKeyDown(object sender, KeyEventArgs e)
        {
            simpleConfigPage.gamepadMappingController.Handler_OnKeyDown(sender, e);
        }
    }
}

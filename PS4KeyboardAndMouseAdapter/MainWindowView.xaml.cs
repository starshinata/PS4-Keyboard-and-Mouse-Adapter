using System;
using System.Diagnostics;
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

        private Process GetRemotePlayProcess() {
            if (vm == null) return null;
            if (vm.RemotePlayInjector == null) return null;

            return vm.RemotePlayInjector.RemotePlayProcess;
        }

        private void MainWindowView_OnActivated(object sender, EventArgs e)
        {
            Process Process = GetRemotePlayProcess();
            if (Process != null)
            {
                Process.Refresh();
            }
        }

        private void MainWindowView_OnDeactivated(object sender, EventArgs e)
        {
            Process Process = GetRemotePlayProcess();
            if (Process != null)
            {
                Process.Refresh();
            }
        }

        public void MainWindowView_OnKeyDown(object sender, KeyEventArgs e)
        {
            simpleConfigPage.gamepadMappingController.Handler_OnKeyDown(sender, e);
        }
    }
}

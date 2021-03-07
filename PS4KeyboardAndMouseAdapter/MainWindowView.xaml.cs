using PS4KeyboardAndMouseAdapter.Config;
using System;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace PS4KeyboardAndMouseAdapter
{

    public partial class MainWindowView
    {

        public MainWindowView()
        {
            InitializeComponent();
            KeyDown += MainWindowView_OnKeyDown;
        }

        private void MainWindowView_OnActivated(object sender, EventArgs e)
        {
            RefreshRemotePlayProcess();
        }

        private void MainWindowView_OnDeactivated(object sender, EventArgs e)
        {
            RefreshRemotePlayProcess();
        }

        public void MainWindowView_OnKeyDown(object sender, KeyEventArgs e)
        {
            simpleConfigPage.gamepadMappingController.Handler_OnKeyDown(sender, e);
        }


        private void RefreshRemotePlayProcess()
        {
            if (InstanceSettings.GetInstance().RemotePlayProcess != null)
            {
                InstanceSettings.GetInstance().RemotePlayProcess.Refresh();
            }
        }
    }
}

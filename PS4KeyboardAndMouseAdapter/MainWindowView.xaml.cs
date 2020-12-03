using System;

namespace PS4KeyboardAndMouseAdapter
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView
    {
        public MainViewModel vm;

        public MainWindowView()
        {
            InitializeComponent();
            vm = (MainViewModel)DataContext;
        }
      
        private void MainWindowView_OnActivated(object sender, EventArgs e)
        {
            vm.RemotePlayProcess.Refresh();
        }

        private void MainWindowView_OnDeactivated(object sender, EventArgs e)
        {
            vm.RemotePlayProcess.Refresh();
        }

    }
}

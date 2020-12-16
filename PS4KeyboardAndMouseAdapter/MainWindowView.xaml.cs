using System;
using System.Reflection;
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

        private static string GetAssemblyVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
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

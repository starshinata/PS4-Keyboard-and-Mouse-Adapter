using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PS4RemotePlayInjection;
using PS4RemotePlayInterceptor;

namespace PS4KeyboardAndMouseAdapter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppExit(object sender, ExitEventArgs e)
        {
            Utility.ShowCursor(true);
            
            //TODO: hardcoded, fix.
            Injector.FindProcess("RemotePlay").Kill();
        }
    }
}

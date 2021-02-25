using System;
using System.Reflection;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;

namespace PS4KeyboardAndMouseAdapter
{

    public class MainViewModel
    {
        public InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();
        
        public UserSettings UserSettings { get; set; } = UserSettings.GetInstance();

        public RemotePlayInjector RemotePlayInjector;

        public string WindowTitle { get; set; } = "PS4 Keyboard&Mouse Adapter v" + GetAssemblyVersion();

        public MainViewModel()
        {
            Log.Information("MainViewModel constructor IN");

            GamepadProcessor gp = new GamepadProcessor();
            RemotePlayInjector = new RemotePlayInjector(gp);
            RemotePlayInjector.OpenRemotePlayAndInject();
            
            Log.Information("MainViewModel constructor OUT");
        }

        public static string GetAssemblyVersion()
        {
            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }
    
        public void RefreshData()
        {
            InstanceSettings.BroadcastRefresh();
            UserSettings.BroadcastRefresh();
        }

    }
}

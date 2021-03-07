using PS4KeyboardAndMouseAdapter.Config;
using Serilog;

namespace PS4KeyboardAndMouseAdapter
{

    public class MainViewModel
    {
        public InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();

        public UserSettings UserSettings { get; set; } = UserSettings.GetInstance();

        public RemotePlayInjector RemotePlayInjector;

        public string WindowTitle { get; set; } = "PS4 Keyboard&Mouse Adapter v" + VersionUtil.GetVersion();

        public MainViewModel()
        {
            Log.Information("MainViewModel constructor IN");

            GamepadProcessor gp = new GamepadProcessor();
            RemotePlayInjector = new RemotePlayInjector(gp);
            RemotePlayInjector.OpenRemotePlayAndInject();

            Log.Information("MainViewModel constructor OUT");
        }

        public void RefreshData()
        {
            InstanceSettings.BroadcastRefresh();
            UserSettings.BroadcastRefresh();
        }

    }
}

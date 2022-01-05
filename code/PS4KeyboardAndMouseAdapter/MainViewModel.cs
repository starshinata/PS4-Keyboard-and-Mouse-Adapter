using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;

namespace Pizza.KeyboardAndMouseAdapter
{

    public class MainViewModel
    {
        public InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();

        public UserSettings UserSettings { get; set; } = UserSettings.GetInstance();

        public GamepadProcessor GamepadProcessor;

        public string WindowTitle { get; set; } = "PS4 Keyboard&Mouse Adapter v" + VersionUtil.GetVersion();

        public MainViewModel()
        {
            Log.Information("MainViewModel constructor IN");
            GamepadProcessor = new GamepadProcessor();
            Log.Information("MainViewModel constructor OUT");
        }

        public void RefreshData()
        {
            Log.Debug("MainViewModel RefreshData IN");
            InstanceSettings.BroadcastRefresh();
            UserSettings.BroadcastRefresh();
            Log.Debug("MainViewModel RefreshData IN");
        }

    }
}

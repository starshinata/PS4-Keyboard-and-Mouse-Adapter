﻿using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;

namespace Pizza.KeyboardAndMouseAdapter
{

    public class MainViewModel
    {
        public ApplicationSettings ApplicationSettings { get; set; } = ApplicationSettings.GetInstance();

        public InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();

        public UserSettingsV3 UserSettings { get; set; } = UserSettingsContainer.GetInstance();

        public GamepadProcessor GamepadProcessor;

        public string WindowTitle { get; set; } = "PS4 Keyboard&Mouse Adapter v" + VersionUtil.GetVersion();

        public MainViewModel()
        {
            GamepadProcessor = new GamepadProcessor();
        }

        public void RefreshData()
        {
            InstanceSettings.BroadcastRefresh();
            UserSettingsContainer.BroadcastRefresh();
        }

    }
}

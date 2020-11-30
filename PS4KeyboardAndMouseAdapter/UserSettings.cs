using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Serilog;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter
{

    public class UserSettings
    {

        public static string PROFILE_DEFAULT = "profiles/default-profile.json";
        public static string PROFILE_PREVIOUS = "profile-previous.json";

        private static UserSettings thisInstance = null;
        private static ILogger staticLogger = Log.ForContext(typeof(UserSettings));

        //////////////////////////////////////////////////////////////////////

        public static UserSettings GetInstance()
        {
            if (thisInstance == null)
            {
                thisInstance = new UserSettings();
            }
            return thisInstance;
        }

        public static void Load(string file)
        {
            staticLogger.Information("UserSettings.Load: " + file);
            thisInstance = ReadUserSettings(file);
        }

        public static void LoadWithCatch(string file)
        {
            try
            {
                Load(file);
            }
            catch (Exception ex)
            {
                staticLogger.Error("UserSettings.LoadWithCatch failed: " + ex.Message);
                staticLogger.Error(ex.GetType().ToString());
                staticLogger.Error(ex.StackTrace);
            }
        }

        public static void LoadDefault()
        {
            LoadWithCatch(PROFILE_DEFAULT);
        }

        public static void LoadPrevious()
        {
            LoadWithCatch(PROFILE_PREVIOUS);
        }

        private static UserSettings ReadUserSettings(string file)
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<UserSettings>(json);
        }


        public static void Save(string file)
        {
            staticLogger.Information("UserSettings.Save: " + file);
            WriteUserSettings(thisInstance, file);
        }

        private static void WriteUserSettings(UserSettings Settings, string file)
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        //////////////////////////////////////////////////////////////////////

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; set; }

        public int AnalogStickLowerRange { get; set; } = 40;
        public int AnalogStickUpperRange { get; set; } = 95;

        public double MouseDistanceLowerRange { get; set; } = 5;
        public double MouseDistanceUpperRange { get; set; } = VideoMode.DesktopMode.Width / 20f;
        public double MouseMaxDistance => VideoMode.DesktopMode.Width / 2f;

        public bool MouseControlsL3 { get; set; } = false;
        public bool MouseControlsR3 { get; set; } = false;


        private int _MousePollingRate;

        // DualShock 4 polling rate
        // 60  Hz Bluetooth
        // 250 Hz Wired
        // so it doesnt make sense for mouse polling rate to be above 250
        public int MousePollingRate
        {
            get
            {
                return _MousePollingRate;
            }
            set
            {
                if (value < 10)
                {
                    _MousePollingRate = 10;
                }
                else if (value > 250)
                {

                    _MousePollingRate = 250;
                }
                else
                {
                    _MousePollingRate = value;
                }
            }
        }


        public double MouseXAxisSensitivityMax { get; set; } = 100;
        public double MouseXAxisSensitivityModifier { get; set; } = 2;
        public double MouseYAxisSensitivityMax { get; set; } = 100;
        public double MouseYAxisSensitivityModifier { get; set; } = 1;


        //TODO do we still need this ?
        public double XYRatio { get; set; } = 0.6;

        private UserSettings()
        {
            MousePollingRate = 60;
        }

    }

}

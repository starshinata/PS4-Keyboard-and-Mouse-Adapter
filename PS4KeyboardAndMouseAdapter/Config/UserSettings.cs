using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public class UserSettings : INotifyPropertyChanged
    {

        public static string PROFILE_DEFAULT = "profiles/default-profile.json";
        public static string PROFILE_PREVIOUS = "profile-previous.json";

        private static UserSettings thisInstance = new UserSettings();
        private static ILogger staticLogger = Log.ForContext(typeof(UserSettings));

        //////////////////////////////////////////////////////////////////////

        public static UserSettings GetInstance()
        {
            return thisInstance;
        }

        public static bool IsLegacyConfig(string json)
        {
            try
            {
                JObject newSetting2s = JsonConvert.DeserializeObject<JObject>(json);

                foreach (JProperty property in newSetting2s.Properties())
                {
                    try
                    {
                        if (property.Name == "Version_1_0_12_OrGreater")
                        {
                            bool value = (bool) property.Value;
                            return value;
                        }
                    }
                    catch (Exception ex)
                    {
                        staticLogger.Error("UserSettings.IsLegacyConfig error(a): " + ex.Message);
                        staticLogger.Error(ex.GetType().ToString());
                        staticLogger.Error(ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                staticLogger.Error("UserSettings.IsLegacyConfig error(b): " + ex.Message);
                staticLogger.Error(ex.GetType().ToString());
                staticLogger.Error(ex.StackTrace);
            }

            return true;
        }

        public static void Load(string file)
        {
            Console.WriteLine("UserSettings.Load: " + file);
            staticLogger.Information("UserSettings.Load: " + file);
            thisInstance.ImportValues(file);

            thisInstance.PropertyChanged(thisInstance, new PropertyChangedEventArgs(""));
            Print(thisInstance);
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
            Print(thisInstance);
        }

        public static void Print(UserSettings settings)
        {
            staticLogger.Information("UserSettings.Print()");
            Console.WriteLine("UserSettings.Print()");

            Console.WriteLine("mappings");
            foreach (VirtualKey key in settings.Mappings.Keys)
            {
                staticLogger.Information("print Mappings:{VirtKey:" + key + ", keyboardValue: " + settings.Mappings[key] + "}");
                Console.WriteLine("print Mappings:{VirtKey:" + key + ", keyboardValue: " + settings.Mappings[key] + "}");
            }


            Console.WriteLine("values");

            Type t = settings.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name != "Mappings")
                {
                    MethodInfo getter = prop.GetGetMethod();
                    if (getter != null)
                    {
                        var value = getter.Invoke(settings, new object[] { });

                        staticLogger.Information("print " + prop + ":" + value);
                        Console.WriteLine("print " + prop + ":" + value);
                    }
                }
            }
        }

        public static void Save(string file)
        {
            staticLogger.Information("UserSettings.Save: " + file);
            string json = JsonConvert.SerializeObject(thisInstance, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public static void SetMapping(VirtualKey key, PhysicalKey value)
        {
            staticLogger.Information("MainViewModel.SetMapping {VirtualKey:" + key + ", PhysicalKey: " + value + "}");

            thisInstance.Mappings[key] = value;

            Save(PROFILE_PREVIOUS);
            thisInstance.PropertyChanged(thisInstance, new PropertyChangedEventArgs(""));
        }

        //////////////////////////////////////////////////////////////////////

        //
        // Instance properties not to be persisted
        //
        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        //
        // Instance properties to be persisted
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        // false if we need to migrate
        // true means we can ignore
        // default if false until we find a value
        public bool Version_1_0_12_OrGreater { get; set; } = false;

        public Dictionary<VirtualKey, PhysicalKey> Mappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();

        public int AnalogStickLowerRange { get; set; } = 40;
        public int AnalogStickUpperRange { get; set; } = 95;

        public bool MouseControlsL3 { get; set; } = false;
        public bool MouseControlsR3 { get; set; } = false;

        public double MouseDistanceLowerRange { get; set; } = 5;
        public double MouseDistanceUpperRange { get; set; } = VideoMode.DesktopMode.Width / 20f;
        public double MouseMaxDistance { get; set; } = VideoMode.DesktopMode.Width / 2f;

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


        public bool MouseAimSensitivityEnabled { get; set; } = false;

        public double MouseXAxisSensitivityAimModifier { get; set; } = 1;
        public double MouseXAxisSensitivityLookModifier { get; set; } = 1;
        public double MouseXAxisSensitivityMax { get; set; } = 100;

        public double MouseYAxisSensitivityAimModifier { get; set; } = 1;
        public double MouseYAxisSensitivityLookModifier { get; set; } = 1;
        public double MouseYAxisSensitivityMax { get; set; } = 100;

        //TODO do we still need this ?
        public double XYRatio { get; set; } = 0.6;

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        //////////////////////////////////////////////////////////////////////

        private UserSettings()
        {
            MousePollingRate = 60;
        }

        public void ImportValues(string file)
        {
            Console.WriteLine("ImportValues()");
            string json = File.ReadAllText(file);
            UserSettings newSettings = null;

            if (IsLegacyConfig(json))
            {
                UserSettings_1_0_11 legacySettings = JsonConvert.DeserializeObject<UserSettings_1_0_11>(json);
                newSettings = UserSettings_1_0_11.ImportValues(legacySettings);
            }

            ImportValuesCurrent(newSettings);
        }

        public static void ImportValuesCurrent(UserSettings newSettings)
        {
            staticLogger.Information("UserSettings.ImportValuesCurrent()");

            //reminder we want to import stuff into variable **thisInstance**

            thisInstance.AnalogStickLowerRange = newSettings.AnalogStickLowerRange;
            thisInstance.AnalogStickUpperRange = newSettings.AnalogStickUpperRange;

            thisInstance.MouseControlsL3 = newSettings.MouseControlsL3;
            thisInstance.MouseControlsR3 = newSettings.MouseControlsR3;

            thisInstance.MouseDistanceLowerRange = newSettings.MouseDistanceLowerRange;
            thisInstance.MouseDistanceUpperRange = newSettings.MouseDistanceUpperRange;
            thisInstance.MouseMaxDistance = newSettings.MouseMaxDistance;

            thisInstance.MousePollingRate = newSettings.MousePollingRate;

            thisInstance.MouseXAxisSensitivityAimModifier = newSettings.MouseXAxisSensitivityAimModifier;
            thisInstance.MouseXAxisSensitivityLookModifier = newSettings.MouseXAxisSensitivityLookModifier;
            thisInstance.MouseXAxisSensitivityMax = newSettings.MouseXAxisSensitivityMax;

            thisInstance.MouseYAxisSensitivityAimModifier = newSettings.MouseYAxisSensitivityAimModifier;
            thisInstance.MouseYAxisSensitivityLookModifier = newSettings.MouseYAxisSensitivityLookModifier;
            thisInstance.MouseYAxisSensitivityMax = newSettings.MouseYAxisSensitivityMax;

            thisInstance.XYRatio = newSettings.XYRatio;


            foreach (VirtualKey key in newSettings.Mappings.Keys)
            {
                thisInstance.Mappings[key] = newSettings.Mappings[key];
            }

            thisInstance.Version_1_0_12_OrGreater = true;
        }

    }
}

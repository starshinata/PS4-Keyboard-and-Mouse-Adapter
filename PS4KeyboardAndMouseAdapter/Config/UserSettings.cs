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

        private static UserSettings ThisInstance = new UserSettings();
        private static ILogger StaticLogger = Log.ForContext(typeof(UserSettings));

        //////////////////////////////////////////////////////////////////////

        public static void BroadcastRefresh()
        {
            ThisInstance.GetKeyboardMappings();
            ThisInstance.PropertyChanged(ThisInstance, new PropertyChangedEventArgs(""));
        }

        public static UserSettings GetInstance()
        {
            return ThisInstance;
        }

        public static void ImportValues(string file)
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
            StaticLogger.Information("UserSettings.ImportValuesCurrent()");

            //reminder we want to import stuff into variable **thisInstance**

            ThisInstance.AnalogStickLowerRange = newSettings.AnalogStickLowerRange;
            ThisInstance.AnalogStickUpperRange = newSettings.AnalogStickUpperRange;

            ThisInstance.MouseControlsL3 = newSettings.MouseControlsL3;
            ThisInstance.MouseControlsR3 = newSettings.MouseControlsR3;

            ThisInstance.MouseDistanceLowerRange = newSettings.MouseDistanceLowerRange;
            ThisInstance.MouseDistanceUpperRange = newSettings.MouseDistanceUpperRange;
            ThisInstance.MouseMaxDistance = newSettings.MouseMaxDistance;

            ThisInstance.MousePollingRate = newSettings.MousePollingRate;

            ThisInstance.MouseXAxisSensitivityAimModifier = newSettings.MouseXAxisSensitivityAimModifier;
            ThisInstance.MouseXAxisSensitivityLookModifier = newSettings.MouseXAxisSensitivityLookModifier;
            ThisInstance.MouseXAxisSensitivityMax = newSettings.MouseXAxisSensitivityMax;

            ThisInstance.MouseYAxisSensitivityAimModifier = newSettings.MouseYAxisSensitivityAimModifier;
            ThisInstance.MouseYAxisSensitivityLookModifier = newSettings.MouseYAxisSensitivityLookModifier;
            ThisInstance.MouseYAxisSensitivityMax = newSettings.MouseYAxisSensitivityMax;

            ThisInstance.XYRatio = newSettings.XYRatio;

            var virtualKeys = KeyUtility.GetVirtualKeyValues();
            Console.WriteLine("virtualKeys " + virtualKeys);
            foreach (VirtualKey key in virtualKeys)
            {
                if (newSettings.Mappings[key] != null)
                {
                    ThisInstance.Mappings[key] = newSettings.Mappings[key];
                }
            }

            ThisInstance.Version_1_0_12_OrGreater = true;
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
                            bool value = (bool)property.Value;
                            return value;
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticLogger.Error("UserSettings.IsLegacyConfig error(a): " + ex.Message);
                        StaticLogger.Error(ex.GetType().ToString());
                        StaticLogger.Error(ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticLogger.Error("UserSettings.IsLegacyConfig error(b): " + ex.Message);
                StaticLogger.Error(ex.GetType().ToString());
                StaticLogger.Error(ex.StackTrace);
            }

            return true;
        }

        public static void Load(string file)
        {
            string fullFilePath = Path.GetFullPath(file);
            Console.WriteLine("UserSettings.Load: " + fullFilePath);
            StaticLogger.Information("UserSettings.Load: " + fullFilePath);
            ImportValues(fullFilePath);

            ThisInstance.GetKeyboardMappings();
            ThisInstance.PropertyChanged(ThisInstance, new PropertyChangedEventArgs(""));
            Print(ThisInstance);
        }

        public static void LoadWithCatch(string file)
        {
            try
            {
                Load(file);
            }
            catch (Exception ex)
            {
                StaticLogger.Error("UserSettings.LoadWithCatch failed: " + ex.Message);
                StaticLogger.Error(ex.GetType().ToString());
                StaticLogger.Error(ex.StackTrace);
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

        public static void Print(UserSettings settings)
        {
            StaticLogger.Information("UserSettings.Print()");
            Console.WriteLine("UserSettings.Print()");

            Console.WriteLine("mappings");
            var virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                StaticLogger.Information("print Mappings:{VirtKey:" + key + ", keyboardValue: " + settings.Mappings[key] + "}");
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

                        StaticLogger.Information("print " + prop + ":" + value);
                        Console.WriteLine("print " + prop + ":" + value);
                    }
                }
            }
        }

        public static void Save(string file)
        {
            StaticLogger.Information("UserSettings.Save: " + file);
            string json = JsonConvert.SerializeObject(ThisInstance, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public static void SetMapping(VirtualKey key, PhysicalKey value)
        {
            StaticLogger.Information("MainViewModel.SetMapping {VirtualKey:" + key + ", PhysicalKey: " + value + "}");

            ThisInstance.Mappings[key] = value;

            Save(PROFILE_PREVIOUS);
            ThisInstance.PropertyChanged(ThisInstance, new PropertyChangedEventArgs(""));
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



        public Dictionary<VirtualKey, PhysicalKey> KeyboardMappings { get; set; } = new Dictionary<VirtualKey, PhysicalKey>();

        public void GetKeyboardMappings()
        {
            var virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                PhysicalKey pk = Mappings[key];
                if (pk != null && pk.KeyboardValue != Keyboard.Key.Unknown)
                {
                    KeyboardMappings[key] = Mappings[key];
                }
            }
        }
    }
}


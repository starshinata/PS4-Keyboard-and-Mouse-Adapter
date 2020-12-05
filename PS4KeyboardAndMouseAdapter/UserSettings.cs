using Newtonsoft.Json;
using Serilog;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace PS4KeyboardAndMouseAdapter
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

        public static void Load(string file)
        {
            staticLogger.Information("UserSettings.Load: " + file);
            thisInstance.ImportValues(ReadFile(file));

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
        }

        public static void Print(UserSettings settings)
        {
            staticLogger.Information("UserSettings.Print()");
            Console.WriteLine("UserSettings.Print()");

            foreach (VirtualKey key in settings.Mappings.Keys)
            {
                staticLogger.Information("print Mappings:{VirtKey:" + key + ", keyboardValue: " + settings.Mappings[key] + "}");
                Console.WriteLine("print Mappings:{VirtKey:" + key + ", keyboardValue: " + settings.Mappings[key] + "}");
            }


            Type t = settings.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                MethodInfo getter = prop.GetGetMethod();
                if (getter != null)
                {
                    Object value = getter.Invoke(settings, new object[] { });

                    staticLogger.Information("print " + prop + ":" + value);
                    Console.WriteLine("print " + prop + ":" + value);
                }
            }

        }

        private static UserSettings ReadFile(string file)
        {
            string json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<UserSettings>(json);
        }

        public static void Save(string file)
        {
            staticLogger.Information("UserSettings.Save: " + file);
            WriteFile(thisInstance, file);
        }

        public static void SetMapping(VirtualKey key, Keyboard.Key value)
        {
            staticLogger.Information("MainViewModel.SetMapping {VirtKey:" + key + ", keyboardValue: " + value + "}");

            thisInstance.Mappings[key] = value;

            Save(PROFILE_PREVIOUS);
            thisInstance.PropertyChanged(thisInstance, new PropertyChangedEventArgs(""));
        }

        private static void WriteFile(UserSettings Settings, string file)
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(file, json);
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

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; set; } = new Dictionary<VirtualKey, Keyboard.Key>();

        public int AnalogStickLowerRange { get; set; } = 40;
        public int AnalogStickUpperRange { get; set; } = 95;

        public bool MouseControlsL3 { get; set; } = false;
        public bool MouseControlsR3 { get; set; } = false;

        public double MouseDistanceLowerRange { get; set; } = 5;
        public double MouseDistanceUpperRange { get; set; } = VideoMode.DesktopMode.Width / 20f;
        public double MouseMaxDistance { get; set; } =  VideoMode.DesktopMode.Width / 2f;

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

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        //////////////////////////////////////////////////////////////////////

        private UserSettings()
        {
            MousePollingRate = 60;
        }

        public void ImportValues(UserSettings newSettings)
        {
            //reminder we want to import stuff into variable **thisInstance**

            thisInstance.AnalogStickLowerRange = newSettings.AnalogStickLowerRange;
            thisInstance.AnalogStickUpperRange = newSettings.AnalogStickUpperRange;

            thisInstance.MouseControlsL3 = newSettings.MouseControlsL3;
            thisInstance.MouseControlsR3 = newSettings.MouseControlsR3;

            thisInstance.MouseDistanceLowerRange = newSettings.MouseDistanceLowerRange;
            thisInstance.MouseDistanceUpperRange = newSettings.MouseDistanceUpperRange;
            thisInstance.MouseMaxDistance = newSettings.MouseMaxDistance;
            
            thisInstance.MousePollingRate = newSettings.MousePollingRate;

            thisInstance.MouseXAxisSensitivityMax = newSettings.MouseXAxisSensitivityMax;
            thisInstance.MouseXAxisSensitivityModifier = newSettings.MouseXAxisSensitivityModifier;
            thisInstance.MouseYAxisSensitivityMax = newSettings.MouseYAxisSensitivityMax;
            thisInstance.MouseYAxisSensitivityModifier = newSettings.MouseYAxisSensitivityModifier;

            thisInstance.XYRatio = newSettings.XYRatio;


            foreach (VirtualKey key in newSettings.Mappings.Keys)
            {
                thisInstance.Mappings[key] = newSettings.Mappings[key];
            }
        }

    }
}

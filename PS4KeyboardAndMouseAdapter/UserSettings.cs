using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Serilog;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter
{

    public class UserSettings : INotifyPropertyChanged
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
            Console.WriteLine("UserSettings.Load: " + file);
            thisInstance.importValues( ReadUserSettings(file));

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
            // ensure to ensure there is something to load into
            GetInstance();

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

        public event PropertyChangedEventHandler PropertyChanged;

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; set; } = new Dictionary<VirtualKey, Keyboard.Key>();

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


        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            Console.WriteLine("User.NotifyPropertyChanged");
            Log.Information("User.NotifyPropertyChanged");
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                Console.WriteLine("User.NotifyPropertyChanged bb");
                Log.Information("User.NotifyPropertyChanged bb");
            }
        }

        // pancakeslp 2020.12.02
        // yes we are using reflection, 
        // and yes it is said to be slow and bad
        // I dont want to write something specific to import values, and then we keep forgetting to update this method
        public void importValues(UserSettings newSettings)
        {
            //reminder we want to import stuff into variable **thisInstance**
            Console.WriteLine("importValues()" );

            Console.WriteLine("importValues old " + thisInstance.ToString());
            Console.WriteLine("importValues old " + thisInstance.Mappings.ToString());

            Type t = newSettings.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                Console.WriteLine("got prop " + prop.Name);
                MethodInfo getter = prop.GetGetMethod();
                MethodInfo setter = prop.GetSetMethod();

                if (getter != null && setter != null) {
                    Console.WriteLine("got getter and setter ");
                    Object value = getter.Invoke(thisInstance, new object[] { });
                    Console.WriteLine("value" + value);
                    setter.Invoke(thisInstance, new object[] { value });
                    Console.WriteLine("value set!");
                }
            }



            Console.WriteLine("importValues old " + thisInstance.ToString());
            Console.WriteLine("importValues old "+ thisInstance.Mappings.ToString());
            Console.WriteLine("importValues new " + newSettings.Mappings.ToString());

            foreach (VirtualKey key in newSettings.Mappings.Keys)
            {
                Console.WriteLine("importValues {VirtKey:" + key + ", keyboardValue: " + newSettings.Mappings[key] + "}");

                thisInstance.Mappings[key] = newSettings.Mappings[key];
            }

            NotifyPropertyChanged();
        }
    }

}

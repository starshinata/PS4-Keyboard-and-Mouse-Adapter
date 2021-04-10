using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public partial class UserSettings : INotifyPropertyChanged
    {

        public static string PROFILE_DEFAULT = "profiles/default-profile.json";
        public static string PROFILE_PREVIOUS = "profile-previous.json";

        private static UserSettings ThisInstance = new UserSettings();
        private static readonly ILogger StaticLogger = Log.ForContext(typeof(UserSettings));


        ////////////////////////////////////////////////////////////////////////////


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
            string json = File.ReadAllText(file);

            UserSettings newSettings;
            if (IsLegacyConfig(json))
            {
                UserSettings_1_0_11 legacySettings = JsonConvert.DeserializeObject<UserSettings_1_0_11>(json);
                newSettings = UserSettings_1_0_11.ImportValues(legacySettings);
            }
            else
            {
                newSettings = JsonConvert.DeserializeObject<UserSettings>(json);
            }

            ImportValuesCurrent(newSettings);
        }

        public static void ImportValuesCurrent(UserSettings newSettings)
        {
            StaticLogger.Information("UserSettings.ImportValuesCurrent()");

            //reminder we want to import stuff into variable **thisInstance**

            ThisInstance.AimToggle = newSettings.AimToggle;
            ThisInstance.AimToggleRetoggleDelay = newSettings.AimToggleRetoggleDelay;

            ThisInstance.AnalogStickLowerRange = newSettings.AnalogStickLowerRange;
            ThisInstance.AnalogStickUpperRange = newSettings.AnalogStickUpperRange;

            ThisInstance.MouseAimSensitivityEnabled = newSettings.MouseAimSensitivityEnabled;

            ThisInstance.MouseControlsL3 = newSettings.MouseControlsL3;
            ThisInstance.MouseControlsR3 = newSettings.MouseControlsR3;

            ThisInstance.MouseDistanceLowerRange = newSettings.MouseDistanceLowerRange;
            ThisInstance.MouseDistanceUpperRange = newSettings.MouseDistanceUpperRange;
            ThisInstance.MouseMaxDistance = newSettings.MouseMaxDistance;

            ThisInstance.MousePollingRate = newSettings.MousePollingRate;

            ThisInstance.MouseWheelScrollHoldDuration = newSettings.MouseWheelScrollHoldDuration;

            ThisInstance.MouseXAxisSensitivityAimModifier = newSettings.MouseXAxisSensitivityAimModifier;
            ThisInstance.MouseXAxisSensitivityLookModifier = newSettings.MouseXAxisSensitivityLookModifier;
            ThisInstance.MouseXAxisSensitivityMax = newSettings.MouseXAxisSensitivityMax;

            ThisInstance.MouseYAxisSensitivityAimModifier = newSettings.MouseYAxisSensitivityAimModifier;
            ThisInstance.MouseYAxisSensitivityLookModifier = newSettings.MouseYAxisSensitivityLookModifier;
            ThisInstance.MouseYAxisSensitivityMax = newSettings.MouseYAxisSensitivityMax;

            ThisInstance.RemotePlayVolume = newSettings.RemotePlayVolume;

            ThisInstance.XYRatio = newSettings.XYRatio;

            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                if (newSettings.MappingsContainsKey(key))
                {
                    ThisInstance.Mappings[key] = newSettings.Mappings[key];
                }
            }

            ThisInstance.Version_2_0_0_OrGreater = true;
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
                        if (property.Name == "Version_2_0_0_OrGreater")
                        {
                            // remember to flip this value
                            // if it is 1.0.11 we want to return true
                            // if it is 2.0.0 or greater we want to return false
                            bool value = !(bool)property.Value;
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
            Console.WriteLine("UserSettings.Print()");
            StaticLogger.Information("UserSettings.Print()");

            Console.WriteLine("print mappings");
            StaticLogger.Information("print mappings");
            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                Console.WriteLine("print Mappings:{VirtKey:" + key + ", PhysicalKeyGroup: " + settings.Mappings[key] + "}");
                StaticLogger.Information("print Mappings:{VirtKey:" + key + ", PhysicalKeyGroup: " + settings.Mappings[key] + "}");
            }

            Console.WriteLine("print values");
            StaticLogger.Information("print values");
            Type t = settings.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name != "KeyboardMappings" && prop.Name != "Mappings")
                {
                    MethodInfo getter = prop.GetGetMethod();
                    if (getter != null)
                    {
                        object value = getter.Invoke(settings, new object[] { });

                        Console.WriteLine("print " + prop + ":" + value);
                        StaticLogger.Information("print " + prop + ":" + value);
                    }
                }
            }
        }

        public static void Save(string file)
        {
            StaticLogger.Information("UserSettings.Save: " + file);

            UserSettings instanceForSaving = ThisInstance.Clone();
            // removing KeyboardMappings, as these are generated after each key remapping
            instanceForSaving.KeyboardMappings = null;

            string json = JsonConvert.SerializeObject(instanceForSaving, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public static void SetMapping(VirtualKey key, PhysicalKey valueOld, PhysicalKey valueNew)
        {
            StaticLogger.Information("MainViewModel.SetMapping {VirtualKey:" + key + ", PhysicalKey: '" + valueOld + " -> " + valueNew + "'}");

            if (!ThisInstance.MappingsContainsKey(key))
            {
                ThisInstance.Mappings[key] = new PhysicalKeyGroup();
            }

            if (valueOld != null)
            {
                ThisInstance.Mappings[key].PhysicalKeys.Remove(valueOld);
            }

            ThisInstance.Mappings[key].PhysicalKeys.Add(valueNew);

            Save(PROFILE_PREVIOUS);
            ThisInstance.PropertyChanged(ThisInstance, new PropertyChangedEventArgs(""));
        }

        public static void TestOnly_ResetUserSettings()
        {
            ThisInstance = new UserSettings();
        }
    }
}

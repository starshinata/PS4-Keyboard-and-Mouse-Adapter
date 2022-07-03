using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    public partial class UserSettingsContainer
    {

        public static string PROFILE_DEFAULT = "profiles/default-profile.json";
        public static string PROFILE_PREVIOUS = "profile-previous.json";

        private static UserSettingsV2 ThisInstance = new UserSettingsV2();


        ////////////////////////////////////////////////////////////////////////////


        public static void BroadcastRefresh()
        {
            ThisInstance.GetKeyboardMappings();
            ThisInstance.BroadcastRefresh();
        }

        public static UserSettingsV2 GetInstance()
        {
            return ThisInstance;
        }

        public static void ImportValues(string file)
        {
            string json = File.ReadAllText(file);

            UserSettingsV2 newSettings;
            if (IsVersion2(json))
            {

                newSettings = UserSettingsV2.ImportValues(json);
            }
            else
            {
                newSettings = UserSettingsV1.ImportValues(json);
            }


            ImportValuesCurrent(newSettings);
        }

        public static void ImportValuesCurrent(UserSettingsV2 NewSettings)
        {
            Log.Information("UserSettings.ImportValuesCurrent()");

            //reminder we want to import stuff into variable **ThisInstance**

            ThisInstance.AimToggle = NewSettings.AimToggle;
            ThisInstance.AimToggleRetoggleDelay = NewSettings.AimToggleRetoggleDelay;

            ThisInstance.AnalogStickLowerRange = NewSettings.AnalogStickLowerRange;
            ThisInstance.AnalogStickUpperRange = NewSettings.AnalogStickUpperRange;

            ThisInstance.MouseAimSensitivityEnabled = NewSettings.MouseAimSensitivityEnabled;

            ThisInstance.MouseControlsL3 = NewSettings.MouseControlsL3;
            ThisInstance.MouseControlsR3 = NewSettings.MouseControlsR3;

            ThisInstance.MouseDistanceLowerRange = NewSettings.MouseDistanceLowerRange;
            ThisInstance.MouseDistanceUpperRange = NewSettings.MouseDistanceUpperRange;
            ThisInstance.MouseMaxDistance = NewSettings.MouseMaxDistance;

            ThisInstance.MousePollingRate = NewSettings.MousePollingRate;

            ThisInstance.MouseWheelScrollHoldDuration = NewSettings.MouseWheelScrollHoldDuration;

            ThisInstance.MouseXAxisSensitivityAimModifier = NewSettings.MouseXAxisSensitivityAimModifier;
            ThisInstance.MouseXAxisSensitivityLookModifier = NewSettings.MouseXAxisSensitivityLookModifier;
            ThisInstance.MouseXAxisSensitivityMax = NewSettings.MouseXAxisSensitivityMax;

            ThisInstance.MouseYAxisSensitivityAimModifier = NewSettings.MouseYAxisSensitivityAimModifier;
            ThisInstance.MouseYAxisSensitivityLookModifier = NewSettings.MouseYAxisSensitivityLookModifier;
            ThisInstance.MouseYAxisSensitivityMax = NewSettings.MouseYAxisSensitivityMax;

            ThisInstance.RemotePlayVolume = NewSettings.RemotePlayVolume;

            ThisInstance.XYRatio = NewSettings.XYRatio;

            List<VirtualKey> virtualKeys = KeyUtility.GetVirtualKeyValues();
            foreach (VirtualKey key in virtualKeys)
            {
                if (NewSettings.MappingsContainsKey(key))
                {
                    ThisInstance.Mappings[key] = NewSettings.Mappings[key];
                }
            }

            ThisInstance.Version_2_0_0_OrGreater = true;
        }

        public static bool IsVersion3(string json)
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
                        ExceptionLogger.LogException("UserSettings.IsLegacyConfig error(a)", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("UserSettings.IsLegacyConfig error(b)", ex);
            }

            return true;
        }
        public static bool IsVersion2(string json)
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
                            return (bool)property.Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogException("UserSettings.IsVersion2 error(a)", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("UserSettings.IsVersion2 error(b)", ex);
            }

            return false;
        }

        public static void Load(string file)
        {
            string fullFilePath = Path.GetFullPath(file);
            Log.Debug("UserSettings.load fullFilePath={0}", fullFilePath);

            ImportValues(fullFilePath);

            ThisInstance.GetKeyboardMappings();
            ThisInstance.BroadcastRefresh();
            ThisInstance.Print();
        }

        public static void LoadWithCatch(string file)
        {
            Log.Information("UserSettings.LoadWithCatch file={0}", file);

            try
            {
                Load(file);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("UserSettings.LoadWithCatch failed", ex);
            }
        }

        public static void LoadDefault()
        {
            Log.Information("UserSettings.LoadDefault");
            LoadWithCatch(PROFILE_DEFAULT);
        }

        public static void LoadPrevious()
        {
            Log.Information("UserSettings.LoadPrevious");
            LoadWithCatch(PROFILE_PREVIOUS);
        }


        public static void Save(string file)
        {
            Log.Information("UserSettings.Save: " + file);

            UserSettingsV2 instanceForSaving = ThisInstance.Clone();
            // removing KeyboardMappings, as these are generated after each key remapping
            instanceForSaving.KeyboardMappings = null;

            string json = JsonConvert.SerializeObject(instanceForSaving, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public static void SetMapping(VirtualKey key, PhysicalKey valueOld, PhysicalKey valueNew)
        {
            Log.Information("MainViewModel.SetMapping {VirtualKey:" + key + ", PhysicalKey: '" + valueOld + " -> " + valueNew + "'}");

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
            ThisInstance.BroadcastRefresh();
        }

        public static void TestOnly_ResetUserSettings()
        {
            ThisInstance = new UserSettingsV2();
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using System;
using System.IO;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    // 2022.07.03 pancakeslp
    // This class is intentionally seperate from UserSettinsV1 / UserSettinsV2
    // or whatever the current version is
    // because i would really like UserSettinsVX to be a POJO / POCO
    // its not going to be a POJO /  POCO but i can dream
    public class UserSettingsContainer
    {

        public static string PROFILE_DEFAULT = "profiles/default-profile.json";
        public static string PROFILE_PREVIOUS = "profile-previous.json";

        private static UserSettingsV3 ThisInstance = new UserSettingsV3();
        private static int nextMappingUid = 0;

        ////////////////////////////////////////////////////////////////////////////


        public static void BroadcastRefresh()
        {
            ThisInstance.RefreshOptimisations();
            ThisInstance.BroadcastRefresh();
        }

        public static UserSettingsV3 GetInstance()
        {
            return ThisInstance;
        }

        public static int getNextMappingUid()
        {
            return nextMappingUid++;
        }

        public static void ImportValues(string file)
        {
            string json = File.ReadAllText(file);

            UserSettingsV3 newSettings;

            if (IsVersion3(json))
            {
                newSettings = UserSettingsV3.ImportValues(json);
            }
            else if (IsVersion2(json))
            {
                newSettings = UserSettingsV3.ImportValues(json);
            }
            else
            {
                newSettings = UserSettingsV1.ImportValues(json);
            }


            ImportValuesCurrent(newSettings);
        }

        public static void ImportValuesCurrent(UserSettingsV3 NewSettings)
        {
            Log.Information("UserSettingsContainer.ImportValuesCurrent()");

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

            ThisInstance.Mappings = NewSettings.Mappings;


            ThisInstance.Version = 3;
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
                        ExceptionLogger.LogException("UserSettingsContainer.IsVersion2 error(a)", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("UserSettingsContainer.IsVersion2 error(b)", ex);
            }

            return false;
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
                        if (property.Name == "Version")
                        {

                            int value = (int)property.Value;
                            return value == 3;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionLogger.LogException("UserSettingsContainer.IsVersion3 error(a)", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("UserSettingsContainer.IsVersion3 error(b)", ex);
            }

            return false;
        }

        public static void Load(string file)
        {
            string fullFilePath = Path.GetFullPath(file);
            Log.Debug("UserSettingsContainer.load fullFilePath={0}", fullFilePath);

            ImportValues(fullFilePath);

            ThisInstance.RefreshOptimisations();
            ThisInstance.BroadcastRefresh();
            ThisInstance.Print();
        }

        public static void LoadWithCatch(string file)
        {
            Log.Information("UserSettingsContainer.LoadWithCatch file={0}", file);

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
            Log.Information("UserSettingsContainer.LoadDefault");
            LoadWithCatch(PROFILE_DEFAULT);
        }

        public static void LoadPrevious()
        {
            Log.Information("UserSettingsContainer.Container.LoadPrevious");
            LoadWithCatch(PROFILE_PREVIOUS);
        }

        public static void Save(string file)
        {
            Log.Information("UserSettingsContainer.Container.Save: " + file);

            UserSettingsV3 instanceForSaving = ThisInstance.Clone();
            // removing KeyboardMappings, as these are generated after each key remapping
            instanceForSaving.KeyboardMappings = null;

            string json = JsonConvert.SerializeObject(instanceForSaving, Formatting.Indented);
            File.WriteAllText(file, json);
        }

        public static void SetMapping(VirtualKey key, PhysicalKey valueOld, PhysicalKey valueNew)
        {
            Log.Information("UserSettingsContainer.SetMapping {VirtualKey:" + key + ", PhysicalKey: '" + valueOld + " -> " + valueNew + "'}");
            throw new Exception("UserSettingsContainer.SetMapping line 222 UNDEFINED");

            /*if (!ThisInstance.MappingsContainsKey(key))
            {
                ThisInstance.Mappings[key] = new PhysicalKeyGroup();
            }

            if (valueOld != null)
            {
                ThisInstance.Mappings[key].PhysicalKeys.Remove(valueOld);
            }

            ThisInstance.Mappings[key].PhysicalKeys.Add(valueNew);
*/
            Save(PROFILE_PREVIOUS);
            ThisInstance.BroadcastRefresh();
        }

        public static void TestOnly_ResetUserSettings()
        {
            nextMappingUid = 0;
            ThisInstance = new UserSettingsV3();
        }
    }
}

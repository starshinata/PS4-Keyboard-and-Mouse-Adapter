using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pizza.Common;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;


namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    internal class UserSettingsLoader
    {

        public static void ImportValues(string file)
        {
            string json = File.ReadAllText(file);

            UserSettings newSettings;
            if (IsLegacyConfig(json))
            {
                UserSettingsV1 legacySettings = JsonConvert.DeserializeObject<UserSettingsV1>(json);
                newSettings = UserSettingsV1.ImportValues(legacySettings);
            }
            else
            {
                newSettings = JsonConvert.DeserializeObject<UserSettings>(json);
            }

            ImportValuesCurrent(newSettings);
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

        public static void Load(string file)
        {
            string fullFilePath = Path.GetFullPath(file);
            Log.Debug("UserSettings.load fullFilePath={0}", fullFilePath);

            ImportValues(fullFilePath);

            ThisInstance.GetKeyboardMappings();
            ThisInstance.PropertyChanged(ThisInstance, new PropertyChangedEventArgs(""));
            Print(ThisInstance);
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

    }
}

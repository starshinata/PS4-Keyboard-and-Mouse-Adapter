using System.IO;
using Newtonsoft.Json;

namespace PS4KeyboardAndMouseAdapter
{

    public class UserSettingsManager
    {
        public static UserSettings ReadUserSettings()
        {
            string json = File.ReadAllText("mappings.json");
            return  JsonConvert.DeserializeObject<UserSettings>(json);
        }

        public static void WriteUserSettings(UserSettings Settings)
        {
            string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText("mappings.json", json);
            
        }
    }

}

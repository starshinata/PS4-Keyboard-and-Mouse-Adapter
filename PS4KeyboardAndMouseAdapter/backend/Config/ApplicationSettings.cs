using Newtonsoft.Json;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;

namespace PS4KeyboardAndMouseAdapter.Config
{
    // a collection of settings are not bound to a specific game
    public class ApplicationSettings : INotifyPropertyChanged
    {
        private static readonly string APPLICATION_SETTINGS_FILE = "application-settings.json";

        private static readonly ApplicationSettings ThisInstance = new ApplicationSettings();
        private static readonly ILogger StaticLogger = Log.ForContext(typeof(ApplicationSettings));

        //////////////////////////////////////////////////////////////////////

        public static ApplicationSettings GetInstance()
        {
            return ThisInstance;
        }

        // If you are comparing this method to the one in UserSettings
        // ApplicationSettings is smaller and has less settings
        public static void ImportValues(string file)
        {
            string json = File.ReadAllText(file);

            ApplicationSettings NewSettings = JsonConvert.DeserializeObject<ApplicationSettings>(json);

            ThisInstance.EmulateController = NewSettings.EmulateController;
            ThisInstance.RemotePlayPath = NewSettings.RemotePlayPath;
        }

        public static void Load()
        {
            try
            {
                string fullFilePath = Path.GetFullPath(APPLICATION_SETTINGS_FILE);
                StaticLogger.Information("ApplicationSettings.Load: " + fullFilePath);
                ImportValues(fullFilePath);
            }
            catch (Exception ex)
            {
                StaticLogger.Error("ApplicationSettings.Load failed: " + ex.Message);
                StaticLogger.Error(ex.GetType().ToString());
                StaticLogger.Error(ex.StackTrace);
            }


            if (ThisInstance.RemotePlayPath == null)
            {
                string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                ThisInstance.RemotePlayPath = programFilesX86 + @"\Sony\PS Remote Play\RemotePlay.exe";
            }
        }

        public static void Save()
        {
            string fullFilePath = Path.GetFullPath(APPLICATION_SETTINGS_FILE);
            StaticLogger.Information("ApplicationSettings.Save: " + fullFilePath);

            //Cloned encase we need to make any corrections before its saved
            ApplicationSettings instanceForSaving = ThisInstance.Clone();

            string json = JsonConvert.SerializeObject(instanceForSaving, Formatting.Indented);
            File.WriteAllText(fullFilePath, json);
        }

        //////////////////////////////////////////////////////////////////////

        public ApplicationSettings Clone()
        {
            // cloning by (serilise to string then deserialise)
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return JsonConvert.DeserializeObject<ApplicationSettings>(json);
        }

        //////////////////////////////////////////////////////////////////////
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        public bool EmulateController { get; set; } = false;

        public string RemotePlayPath;

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //
    }
}

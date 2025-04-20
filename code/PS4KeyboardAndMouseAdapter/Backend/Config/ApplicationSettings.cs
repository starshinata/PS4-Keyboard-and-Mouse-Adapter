﻿using Newtonsoft.Json;
using Pizza.Common;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    // a collection of settings are not bound to a specific game
    // BUT are remebered between each run of the application
    public class ApplicationSettings : INotifyPropertyChanged
    {

        #region static
        private static readonly string APPLICATION_SETTINGS_FILE = "application-settings.json";

        private static ApplicationSettings ThisInstance = new ApplicationSettings();
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

            ThisInstance.ColourSchemeIsLight = NewSettings.ColourSchemeIsLight; 
            ThisInstance.EmulateController = NewSettings.EmulateController;
            ThisInstance.EmulationMode = NewSettings.EmulationMode;
            ThisInstance.GamepadUpdaterNoSleep = NewSettings.GamepadUpdaterNoSleep; 
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
                ExceptionLogger.LogException("ApplicationSettings.Load failed", ex);
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

        public string GetAsJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public ApplicationSettings Clone()
        {
            // cloning by (serilise to string then deserialise)
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return JsonConvert.DeserializeObject<ApplicationSettings>(json);
        }

        public static void TestOnly_ResetApplicationSettings()
        {
            ThisInstance = new ApplicationSettings();
        }

        #endregion static


        public event PropertyChangedEventHandler PropertyChanged = delegate { };


        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //

        public bool ColourSchemeIsLight { get; set; } = false;

        public bool EmulateController { get; set; } = false;

        public int EmulationMode;

        public bool GamepadUpdaterNoSleep { get; set; } = false;

        public string RemotePlayPath;

        //
        // REMINDER if you add a new property, be sure to add it to ImportValues method
        //
    }
}

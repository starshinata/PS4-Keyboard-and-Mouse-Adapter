using System;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    public partial class ProfileManager : System.Windows.Controls.UserControl
    {

        public ProfileManager()
        {
            InitializeComponent();
        }

        public void HandleLoad(object sender, RoutedEventArgs e)
        {
            Log.Debug("ProfileManager.HandleLoad");
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true; 
                openFileDialog.DefaultExt = "json";
                openFileDialog.Filter = "JSON file|*.json";

                openFileDialog.ShowDialog();
                if (openFileDialog.FileName != "")
                {
                    UserSettings.Load(openFileDialog.FileName);
                }
            }
            catch (JsonReaderException ex)
            {
                Log.Logger.Error("HandleLoad failed: json read  exception " + ex.Message);
                System.Windows.MessageBox.Show("That file didnt appear to be json!",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Log.Logger.Error("HandleLoad failed: " + ex.Message);
                Log.Logger.Error(ex.GetType().ToString());
                Log.Logger.Error(ex.StackTrace);
            }
        }

        public void HandleReset(object sender, RoutedEventArgs e)
        {
            Log.Debug("ProfileManager.HandleReset");
            UserSettings.LoadDefault();
        }

        public void HandleSave(object sender, RoutedEventArgs e)
        {
            Log.Debug("ProfileManager.HandleSave");
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = "json";
                saveFileDialog.Filter = "JSON file|*.json";

                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "") { 
                    UserSettings.Save(saveFileDialog.FileName);
            }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("HandleSave failed: " + ex.Message);
                Log.Logger.Error(ex.GetType().ToString());
                Log.Logger.Error(ex.StackTrace);
            }
        }

    }
}

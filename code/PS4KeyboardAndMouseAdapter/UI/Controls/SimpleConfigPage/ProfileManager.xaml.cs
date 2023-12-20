using System;
using System.Windows;
using System.Windows.Forms;
using Newtonsoft.Json;
using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls
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
                    UserSettingsContainer.Load(openFileDialog.FileName);
                }
            }
            catch (JsonReaderException ex)
            {
                ExceptionLogger.LogException("ProfileManager.HandleLoad failed: json read exception", ex);

                System.Windows.MessageBox.Show("That file didnt appear to be json!",
                    "Error reading file",
                    (MessageBoxButton)MessageBoxButtons.OK,
                    (MessageBoxImage)MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("ProfileManager.HandleLoad failed: unexpected exception", ex); 
            }
        }

        public void HandleReset(object sender, RoutedEventArgs e)
        {
            Log.Debug("ProfileManager.HandleReset");
            UserSettingsContainer.LoadDefault();
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
                    UserSettingsContainer.Save(saveFileDialog.FileName);
            }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("ProfileManager.HandleSave failed", ex);
            }
        }

    }
}

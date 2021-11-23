using PS4KeyboardAndMouseAdapter.backend;
using PS4KeyboardAndMouseAdapter.backend.DebugLogging;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class PlaystationControl : System.Windows.Controls.UserControl
    {
        public PlaystationControl()
        {
            InitializeComponent();

            ResetErrorMessages();
            RemotePlayTextBox.Text = ApplicationSettings.GetInstance().RemotePlayPath;
            ValidateRewasdUninstalled();
            DetectPlaystationController();
            ValidateRemotePlayPath();
        }

        private void DetectPlaystationController()
        {
            PlaystationControllerDetector.DetectControllers();
            bool isConnected = PlaystationControllerDetector.IsPlaystationControllerConnected();
            if (isConnected)
            {
                ErrorTextBox_PlaystationController.Foreground = UIConstants.TEXTBOX_COLOUR_RED;
                ErrorTextBox_PlaystationController.Text = "Warning: PS4 controller connected!";
                ErrorTextBox_PlaystationController.Visibility = UIConstants.VISIBILITY_VISIBLE;
            }
            else
            {
                ErrorTextBox_PlaystationController.Foreground = UIConstants.TEXTBOX_COLOUR_GREEN;
                ErrorTextBox_PlaystationController.Text = "Success: no PS4 controller connected";
                ErrorTextBox_PlaystationController.Visibility = UIConstants.VISIBILITY_VISIBLE;
            }
        }

        private void Handle_CheckRemoteRemotePlayPath(object sender, RoutedEventArgs e)
        {
            ValidateRemotePlayPath();
        }

        private void Handle_DetectPlaystationController(object sender, RoutedEventArgs e)
        {
            DetectPlaystationController();
        }

        private void Handle_InstallRemotePlay(object sender, RoutedEventArgs e)
        {
            RunRemotePlaySetup();
        }

        private void Handle_LaunchRemotePlay(object sender, RoutedEventArgs e)
        {
            ResetErrorMessages();

            //This one is just a warning, as I suspect that other PS4 controllers might have other Ids
            DetectPlaystationController();

            bool IsGoodRemotePlayPath = ValidateRemotePlayPath();
            bool IsGoodRewasd = ValidateRewasdUninstalled();

            if (!IsGoodRemotePlayPath)
            {
                return;
            }

            if (!IsGoodRewasd)
            {
                return;
            }

            ApplicationSettings.GetInstance().RemotePlayPath = RemotePlayTextBox.Text;

            // if this is before the injection does this finish before injector ?
            DebugDump.Dump();

            Window window = System.Windows.Application.Current.MainWindow;
            ((MainWindowView)window).WelcomeStep1Done();

            GamepadProcessor gp = ((MainViewModel)DataContext).GamepadProcessor;
            RemotePlayInjector RemotePlayInjector = new RemotePlayInjector(gp);
            RemotePlayInjector.OpenRemotePlayAndInject();

        }

        private void Handle_LogFileLocationOpen(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", LogUtility.GetLogDirectoryAbsolute());
        }

        private void Handle_LogLevelChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Combo_LogLevel.SelectedValue != null)
            {
                string value = ((ComboBoxItem)Combo_LogLevel.SelectedValue).Content.ToString();
                if ("Default" == value)
                {
                    ((PS4KeyboardAndMouseAdapter.App)System.Windows.Application.Current).SetLoggingLevel(Serilog.Events.LogEventLevel.Information);
                }
                else if ("Debug" == value)
                {
                    ((PS4KeyboardAndMouseAdapter.App)System.Windows.Application.Current).SetLoggingLevel(Serilog.Events.LogEventLevel.Debug);
                }
                else if ("Verbose" == value)
                {
                    ((PS4KeyboardAndMouseAdapter.App)System.Windows.Application.Current).SetLoggingLevel(Serilog.Events.LogEventLevel.Verbose);
                }
            }
        }

        private void Handle_SetRemoteRemotePlayPath(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.DefaultExt = "exe";
                openFileDialog.Filter = "Executable|*.exe";

                openFileDialog.ShowDialog();
                if (openFileDialog.FileName != "")
                {
                    RemotePlayTextBox.Text = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error("HandleLoad failed: " + ex.Message);
                Log.Logger.Error(ex.GetType().ToString());
                Log.Logger.Error(ex.StackTrace);
            }
        }

        private void ResetErrorMessages()
        {
            ErrorTextBox_PlaystationController.Text = "";
            ErrorTextBox_PlaystationController.Visibility = UIConstants.VISIBILITY_HIDDEN;

            ErrorTextBox_Rewasd.Text = "";
            ErrorTextBox_Rewasd.Visibility = UIConstants.VISIBILITY_HIDDEN;

            ErrorTextBox_RemotePlayPath.Text = "";
            ErrorTextBox_RemotePlayPath.Visibility = UIConstants.VISIBILITY_HIDDEN;
        }

        private void RunRemotePlaySetup()
        {
            string installerName = "RemotePlayInstaller.exe";

            using (WebClient client = new WebClient())
            {
                client.DownloadFile("https://remoteplay.dl.playstation.net/remoteplay/module/win/RemotePlayInstaller.exe", installerName);
            }

            Log.Information("RunRemotePlaySetup - RemotePlay installer started");
            Process installerProcess = Process.Start(installerName);
            installerProcess.EnableRaisingEvents = true;
        }

        /// <returns>
        /// returns true if rewasd is not installed
        /// </returns>
        private bool ValidateRewasdUninstalled()
        {
            string rewasdPath = RewasdDetector.GetExistingRewadPath();
            if (rewasdPath == null)
            {
                ErrorTextBox_Rewasd.Foreground = UIConstants.TEXTBOX_COLOUR_GREEN;
                ErrorTextBox_Rewasd.Text = "Success: reWASD was not detected";
                ErrorTextBox_Rewasd.Visibility = UIConstants.VISIBILITY_VISIBLE;
                return true;
            }
            else
            {
                ErrorTextBox_Rewasd.Foreground = UIConstants.TEXTBOX_COLOUR_RED;
                ErrorTextBox_Rewasd.Text = "Error: reWASD detected at " + rewasdPath;
                ErrorTextBox_Rewasd.Visibility = UIConstants.VISIBILITY_VISIBLE;
                return false;
            }
        }

        /// <returns>
        /// returns true if remote play valid exists
        /// </returns>
        private bool ValidateRemotePlayPath()
        {
            if (File.Exists(RemotePlayTextBox.Text))
            {

                ErrorTextBox_RemotePlayPath.Foreground = UIConstants.TEXTBOX_COLOUR_GREEN;
                ErrorTextBox_RemotePlayPath.Text = "Success: Path exists!";
                ErrorTextBox_RemotePlayPath.Visibility = UIConstants.VISIBILITY_VISIBLE;
                return true;
            }
            else
            {
                ErrorTextBox_RemotePlayPath.Foreground = UIConstants.TEXTBOX_COLOUR_RED;
                ErrorTextBox_RemotePlayPath.Text = "Error: Path doesn't exist!";
                ErrorTextBox_RemotePlayPath.Visibility = UIConstants.VISIBILITY_VISIBLE;
                return false;
            }
        }

    }
}

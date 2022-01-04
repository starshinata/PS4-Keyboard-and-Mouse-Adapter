using Pizza.Common;
using PS4KeyboardAndMouseAdapter.backend;
using PS4KeyboardAndMouseAdapter.backend.DebugLogging;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class WelcomeSetupControl : System.Windows.Controls.UserControl
    {
        public WelcomeSetupControl()
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
                ErrorTextBox_PlaystationController.Text = "Warning: PS controller connected!";
                ErrorTextBox_PlaystationController.Visibility = UIConstants.VISIBILITY_VISIBLE;
            }
            else
            {
                ErrorTextBox_PlaystationController.Foreground = UIConstants.TEXTBOX_COLOUR_GREEN;
                ErrorTextBox_PlaystationController.Text = "Success: no PS controller connected";
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
            RemotePlayInstaller.RunRemotePlaySetup();
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

            DebugDump.Dump();

            Window window = System.Windows.Application.Current.MainWindow;
            ((MainWindowView)window).WelcomeStep1Done_SetupChecked();

            //element defined in xaml
            emulationPickerControl.GetValueAndSaveValueInApplicationSettings();

            //////////////////////////////////////////////////////////////////

            // the below is kinda ASYNC so be sure to do this last
            StartRemotePlayAndConditionallyInject();
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
                ExceptionLogger.LogException("WelcomeSetupControl.Handle_SetRemoteRemotePlayPath failed", ex);
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

        private void StartRemotePlayAndConditionallyInject()
        {

            if (EmulationConstants.ONLY_VIGEM.Equals(ApplicationSettings.GetInstance().EmulationMode))
            {
                RemotePlayStarter rps = new RemotePlayStarter();
                rps.OpenRemotePlay();
            }
            else
            {
                GamepadProcessor gp = ((MainViewModel)DataContext).GamepadProcessor;
                RemotePlayInjector RemotePlayInjector = new RemotePlayInjector();
                RemotePlayInjector.OpenRemotePlayAndInject(gp);
            }
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

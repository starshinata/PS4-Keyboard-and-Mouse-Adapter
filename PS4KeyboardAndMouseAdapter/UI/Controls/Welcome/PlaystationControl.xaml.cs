using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.Welcome
{
    public partial class PlaystationControl : System.Windows.Controls.UserControl
    {
        public PlaystationControl()
        {
            InitializeComponent();

            RemotePlayTextBox.Text = ApplicationSettings.GetInstance().RemotePlayPath;
        }

        private void Handle_LaunchRemotePlay(object sender, RoutedEventArgs e)
        {
            ResetErrorMessages();

            if (!File.Exists(RemotePlayTextBox.Text))
            {
                ErrorTextBox_RemotePlayPath.Text = "Error: Path doesn't exist!";
                ErrorTextBox_RemotePlayPath.Visibility = UIConstants.VisibilityVisible;
                return;
            }

            ApplicationSettings.GetInstance().RemotePlayPath = RemotePlayTextBox.Text;

            GamepadProcessor gp = ((MainViewModel)DataContext).GamepadProcessor;
            Console.WriteLine("gp " + gp);
            RemotePlayInjector RemotePlayInjector = new RemotePlayInjector(gp);
            RemotePlayInjector.OpenRemotePlayAndInject();
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
            ErrorTextBox_RemotePlayPath.Text = "";
            ErrorTextBox_RemotePlayPath.Visibility = UIConstants.VisibilityHidden;
        }
    }
}

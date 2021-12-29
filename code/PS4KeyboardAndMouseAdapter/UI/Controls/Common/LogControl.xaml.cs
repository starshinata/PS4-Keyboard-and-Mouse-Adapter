using PS4KeyboardAndMouseAdapter.backend;
using PS4KeyboardAndMouseAdapter.Config;
using Serilog;
using Serilog.Events;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls.Common
{
    public partial class LogControl : System.Windows.Controls.UserControl
    {

        private readonly int LOG_INDEX_DEFAULT = 0;
        private readonly int LOG_INDEX_DEBUG = 1;
        private readonly int LOG_INDEX_VERBOSE = 2;

        public LogControl()
        {
            InitializeComponent();
            SetInitialLogLevel();
        }

        private void SetInitialLogLevel()
        {
            LogManager logManager = InstanceSettings.GetInstance().GetLogManager();
            LogEventLevel level = logManager.GetLoggingLevel();
            Log.Information("LogControl.SetInitialLogLevel using {0}", level);

            if (LogEventLevel.Debug == level)
            {
                Combo_LogLevel.SelectedIndex = LOG_INDEX_DEBUG;
            }
            else if (LogEventLevel.Verbose == level)
            {
                Combo_LogLevel.SelectedIndex = LOG_INDEX_VERBOSE;
            }
            else
            {
                Combo_LogLevel.SelectedIndex = LOG_INDEX_DEFAULT;
            }
        }

        private void Handle_LogFileLocationOpen(object sender, RoutedEventArgs e)
        {
            LogUtility.LogFileLocationOpen();
        }

        private void Handle_LogLevelChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (Combo_LogLevel.SelectedValue != null)
            {
                LogManager logManager = InstanceSettings.GetInstance().GetLogManager();

                string tag = ((ComboBoxItem)Combo_LogLevel.SelectedItem).Tag.ToString();
                int value = Int32.Parse(tag);
                if (LOG_INDEX_DEFAULT == value)
                {
                    logManager.SetLoggingLevel(LogEventLevel.Information);
                }
                else if (LOG_INDEX_DEBUG == value)
                {
                    logManager.SetLoggingLevel(LogEventLevel.Debug);
                }
                else if (LOG_INDEX_VERBOSE == value)
                {
                    logManager.SetLoggingLevel(LogEventLevel.Verbose);
                }
            }
        }

    }
}

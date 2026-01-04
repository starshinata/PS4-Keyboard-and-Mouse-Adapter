using Pizza.Common;
using Pizza.KeyboardAndMouseAdapter.Backend;
using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Serilog;
using Serilog.Events;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.Common
{
    public partial class LogControl : System.Windows.Controls.UserControl
    {

        public static readonly int INDEX_LOG_DEFAULT = 0;
        public static readonly int INDEX_LOG_DEBUG = 1;
        public static readonly int INDEX_LOG_VERBOSE = 2;

        public static readonly string TAG_LOG_DEFAULT = INDEX_LOG_DEFAULT.ToString();
        public static readonly string TAG_LOG_DEBUG = INDEX_LOG_DEBUG.ToString();
        public static readonly string TAG_LOG_VERBOSE = INDEX_LOG_VERBOSE.ToString();

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
                Combo_LogLevel.SelectedIndex = INDEX_LOG_DEBUG;
            }
            else if (LogEventLevel.Verbose == level)
            {
                Combo_LogLevel.SelectedIndex = INDEX_LOG_VERBOSE;
            }
            else
            {
                Combo_LogLevel.SelectedIndex = INDEX_LOG_DEFAULT;
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
                ComboBoxItem selectedItem = (ComboBoxItem)Combo_LogLevel.SelectedItem;
                if (selectedItem.Tag != null)
                {
                    string tag = selectedItem.Tag.ToString();
                    int value = Int32.Parse(tag);
                    LogManager logManager = InstanceSettings.GetInstance().GetLogManager();

                    if (INDEX_LOG_DEFAULT == value)
                    {
                        logManager.SetLoggingLevel(LogEventLevel.Information);
                    }
                    else if (INDEX_LOG_DEBUG == value)
                    {
                        logManager.SetLoggingLevel(LogEventLevel.Debug);
                    }
                    else if (INDEX_LOG_VERBOSE == value)
                    {
                        logManager.SetLoggingLevel(LogEventLevel.Verbose);
                    }
                }
            }
        }

    }
}

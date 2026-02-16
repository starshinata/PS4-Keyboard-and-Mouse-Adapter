using Pizza.Common;
using Pizza.Common.Gamepad;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    // a collection of settings that will die when application is closed
    // AND ARE NOT PERSISTED ANYWHERE

    public class InstanceSettings : INotifyPropertyChanged
    {
        private static readonly InstanceSettings thisInstance = new InstanceSettings();

        public static void BroadcastRefresh()
        {
            thisInstance.PropertyChanged(thisInstance, new PropertyChangedEventArgs(""));
        }

        public static InstanceSettings GetInstance()
        {
            return thisInstance;
        }

        //////////////////////////////////////////////////////////////////////

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private LogManager logManager = null;
        public bool EnableMouseInput { get; set; } = false;

        // TODO this smells
        // refactor GamepadProcessor to be called without needing to get it via MainViewModel
        public DualShockState DualShockState = null;

        private VigemInternals vigemInternals = null;

        private Process remotePlayProcess = null;

        //////////////////////////////////////////////////////////////////////


        public LogManager GetLogManager()
        {
            return logManager;
        }

        public void SetLogManager(LogManager value)
        {
            logManager = value;
        }

        public Process GetRemotePlayProcess()
        {
            return remotePlayProcess;
        }

        public void SetRemotePlayProcess(Process newProcess)
        {
            remotePlayProcess = newProcess;
            Log.Information("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) set {0} {1}",
                remotePlayProcess.Id, remotePlayProcess.ProcessName);
        }

        public VigemInternals GetVigemInternals()
        {
            return vigemInternals;
        }

        public void SetVigemInternals(VigemInternals value)
        {
            vigemInternals = value;
        }
    }
}


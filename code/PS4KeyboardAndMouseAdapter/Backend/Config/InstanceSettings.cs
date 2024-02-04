using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using PS4RemotePlayInjection;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    // a collection of settings that will die when application is closed
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
            return UtilityData.RemotePlayProcess;
        }

        public void SetRemotePlayProcess(Process newProcess)
        {
            Log.Error("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) set " + newProcess.Id);
            UtilityData.RemotePlayProcess = newProcess;
            UtilityData.pid = newProcess.Id;

            Log.Error("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) get a " + UtilityData.RemotePlayProcess);
            Log.Error("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) get b " + UtilityData.RemotePlayProcess.Id);
            Log.Error("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) get c " + UtilityData.pid);
            Log.Error("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) get d " + Process.GetCurrentProcess());
            Log.Error("InstanceSettings.SetRemotePlayProcess (UtilityData.RemotePlayProcess) get d " + Process.GetCurrentProcess().Id);
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


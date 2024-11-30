using Pizza.KeyboardAndMouseAdapter.Backend.ControllerState;
using Pizza.KeyboardAndMouseAdapter.Backend.Vigem;
using PS4RemotePlayInjection;
using Serilog;
using System;
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

        public bool EnableMouseInput { get; set; } = false;

        public bool EnableOnscreenSticks { get; set; } = false;

        // If not null, you will have a Point
        // in Point you have two Coordinates
        // Each coordinates is a range between -1 and 1
        // 
        // think of
        // -1 as 100% Left
        // 1 as 100% Right
        // -1 as 100% Up
        // 1 as 100% Down
        private NullablePoint LeftStick = null;

        private LogManager logManager = null;

        public bool OnscreenSticksClickRequired { get; set; } = true;

        // see LeftStick for documentation
        private NullablePoint RightStick = null;

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


        public NullablePoint GetLeftStick()
        {
            return LeftStick;
        }

        public NullablePoint GetRightStick()
        {
            return RightStick;
        }

        public void SetStick(string label, NullablePoint stick)
        {
            if (string.Equals("LEFT", label, StringComparison.OrdinalIgnoreCase))
            {
                LeftStick = stick;
            }
            else if (string.Equals("RIGHT", label, StringComparison.OrdinalIgnoreCase))
            {
                RightStick = stick;
            }
            else
            {
                Log.Error("InstanceSettings.SetStick() recieved label " + label);
                throw new Exception("UNKNOWN STICK");
            }
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


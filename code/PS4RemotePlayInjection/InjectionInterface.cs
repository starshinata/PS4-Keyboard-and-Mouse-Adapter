using Pizza.Common;
using Serilog;
using System;
using System.Linq;
using System.Text;

namespace PS4RemotePlayInjection
{
    /// <summary>
    /// Provides an interface for communicating from the client (PsRemotePlay) to the server (injector)
    /// </summary>
    public class InjectionInterface : MarshalByRefObject
    {

        public void LogError(string msg)
        {
            // maybe error is bad when injected?
            Log.Error("ERROR" + msg);
        }

        public void LogError(Exception e, string msg)
        {
            ExceptionLogger.LogException(msg, e);
        }

        public void LogDebug(string msg)
        {
            Log.Debug(msg);
        }

        public void LogInformation(string msg)
        {
            Log.Information(msg);
        }

        public void LogVerbose(string msg)
        {
            Log.Verbose(msg);
        }

        double mapByteTo0to1(byte b)
        {
            return map(b - 128, 0, 255, 0, 1);
        }

        double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        /// <summary>
        /// Called when the hook has been injected successfully
        /// </summary>
        public void OnInjectionSuccess(int clientPID)
        {
            // printing at all four levels to confirm what logging it working
            Log.Error("(NOT AN ERROR) OnInjectionSuccess clientPID {0}", clientPID);
            Log.Verbose("OnInjectionSuccess clientPID {0}", clientPID);
            Log.Debug("OnInjectionSuccess clientPID {0}", clientPID);
            Log.Information("OnInjectionSuccess clientPID {0}", clientPID);
        }

        public void OnReadFile(string filename, ref byte[] inputReport)
        {
            try
            {
                Log.Verbose("InjectionInterface.OnReadFile filename " + filename);

                //if((printFrequency++)%30 == 0)
                //  PrintAnalogSticksAsDegrees(inputReport.Skip(1).Take(4).ToArray());

                // Expect inputReport to be modified
                if (Injector.Callback != null)
                {
                    Log.Verbose("InjectionInterface.OnReadFile Injector.Callback " + Injector.Callback.ToString());
                    // Parse the state
                    DualShockState state = DualShockState.ParseFromDualshockRaw(inputReport);

                    // Skip if state is invalid
                    if (state == null)
                        return;

                    // Expect it to be modified
                    Injector.Callback(ref state);

                    // Convert it back
                    state.ConvertToDualshockRaw(ref inputReport);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException("InjectionInterface.OnReadFile error", ex);
            }
        }

        /// <summary>
        /// Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
            // Store timestamp
            Injector.LastPingTime = DateTime.Now;
            LogVerbose("InjectionInterface.Ping");
        }

        public void PrintAnalogSticksAsDegrees(byte[] bytes)
        {
            bytes = bytes.Skip(1).Take(4).ToArray();
            var sb = new StringBuilder("new byte[] { ");
            //foreach (var b in bytes)
            //{
            //     map(b - 128, 0, 255, 0, 359).ToString().PadLeft(3, ' ') + ", ");
            //}
            sb.Append((int)(Math.Atan2(mapByteTo0to1(bytes[1]), mapByteTo0to1(bytes[0])) * 180d / Math.PI));
            sb.Append(", ");
            sb.Append((int)(Math.Atan2(mapByteTo0to1(bytes[3]), mapByteTo0to1(bytes[2])) * 180d / Math.PI));

            sb.Append("}");
            Log.Debug(sb.ToString());
        }

        public bool ShouldEmulateController()
        {
            return EmulationConstants.ONLY_PROCESS_INJECTION.Equals(Injector.EmulationMode);
        }

        public bool ShouldShowToolbar()
        {
            bool x = (EmulationConstants.ONLY_PROCESS_INJECTION.Equals(Injector.EmulationMode) ||
                EmulationConstants.VIGEM_AND_PROCESS_INJECTION.Equals(Injector.EmulationMode)) &&
                UtilityData.IsToolBarVisible;

            Log.Information("InjectionInterface.ShouldShowToolbar {0}", x);
            return x;
        }
    }
}

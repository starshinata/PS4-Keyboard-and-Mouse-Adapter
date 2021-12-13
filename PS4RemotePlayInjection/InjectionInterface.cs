// PS4RemotePlayInterceptor (File: Classes/InjectionInterface.cs)
//
// Copyright (c) 2018 Komefai
//
// Visit http://komefai.com for more information
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using System.Text;
using PS4RemotePlayInjection;
using Serilog;

namespace PS4RemotePlayInterceptor
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    class InjectionInterface : MarshalByRefObject
    {
        //TODO write something the uses each log level as injector init
        public void LogError(string msg)
        {
            // maybe error is bad when injected?
            Log.Logger.Debug("ERROR" + msg);
        }

        public void LogError(Exception e, string msg)
        {
            // maybe error is bad when injected?
            Log.Logger.Debug("ERROR" + msg);
            Log.Logger.Debug(e, "");
        }

        public void LogDebug(string msg)
        {
            Log.Logger.Debug(msg);
        }

        public void LogInformation(string msg)
        {
            Log.Logger.Information(msg);
        }

        public void LogVerbose(string msg)
        {
            //Log.Logger.Verbose(msg);
        }
        public void LogVerboseForControllerIO(string msg)
        {
            Log.Logger.Verbose(msg);
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
            Log.Logger.Error("(NOT AN ERROR) OnInjectionSuccess clientPID {0}", clientPID);
            Log.Logger.Verbose("OnInjectionSuccess clientPID {0}", clientPID);
            Log.Logger.Debug("OnInjectionSuccess clientPID {0}", clientPID);
            Log.Logger.Information("OnInjectionSuccess clientPID {0}", clientPID);
        }

        public void OnReadFile(string filename, ref byte[] inputReport)
        {
            try
            {
                Log.Logger.Verbose("InjectionInterface.OnReadFile filename " + filename);

                //if((printFrequency++)%30 == 0)
                //  PrintAnalogSticksAsDegrees(inputReport.Skip(1).Take(4).ToArray());

                // Expect inputReport to be modified
                if (Injector.Callback != null)
                {
                    Log.Logger.Verbose("InjectionInterface.OnReadFile Injector.Callback " + Injector.Callback.ToString());
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
                Log.Logger.Error("Problem in OnReadFile: " + ex.Message);
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
            return Injector.EmulateController;
        }

        public bool ShouldShowToolbar()
        {
            bool x = UtilityData.IsToolBarVisible;
            Log.Logger.Information("InjectionInterface.ShouldShowToolbar {0}", x);
            return x;
        }
    }
}

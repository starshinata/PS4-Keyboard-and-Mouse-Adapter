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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PS4RemotePlayInterceptor
{
    /// <summary>
    /// Provides an interface for communicating from the client (target) to the server (injector)
    /// </summary>
    class InjectionInterface : MarshalByRefObject
    {
        public static int printFrequency = 10;

        /// <summary>
        /// Called when the hook has been injected successsfully
        /// </summary>
        public void OnInjectionSuccess(int clientPID)
        {
            try
            {
                Console.WriteLine("OnInjectionSuccess {0}", clientPID);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Called to confirm that the IPC channel is still open / host application has not closed
        /// </summary>
        public void Ping()
        {
            try
            {
                //Console.WriteLine("Ping");

                // Store timestamp
                Injector.LastPingTime = DateTime.Now;
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Report log
        /// </summary>
        /// <param name="message"></param>
        public void ReportLog(string message)
        {
            try
            {
                Console.WriteLine("ReportLog {0}", message);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Report exception
        /// </summary>
        /// <param name="e"></param>
        public void ReportException(Exception e)
        {
            try
            {
                Console.WriteLine("ReportException {0}", e.Message);
            }
            catch (Exception) { }
        }


        /* Interface for hooks */

        public void OnCreateFile(string filename, string mode)
        {
            //Console.WriteLine("OnCreateFile {0} | {1}", filename, mode);
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
            Console.WriteLine(sb.ToString());
        }

        double mapByteTo0to1(byte b)
        {
            return map(b - 128, 0, 255, 0, 1);
        }

        double map(double x, double in_min, double in_max, double out_min, double out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public void OnReadFile(string filename, ref byte[] inputReport)
        {
            try
            {
                //Console.WriteLine("OnReadFile {0}", filename);
                //if((printFrequency++)%30 == 0)
                  //  PrintAnalogSticksAsDegrees(inputReport.Skip(1).Take(4).ToArray());
                // Expect inputReport to be modified
                if (Injector.Callback != null)
                {
                    // Parse the state
                    var state = DualShockState.ParseFromDualshockRaw(inputReport);

                    // Skip if state is invalid
                    if (state == null)
                        return;

                    // Expect it to be modified
                    Injector.Callback(ref state);

                    // Convert it back
                    state.ConvertToDualshockRaw(ref inputReport);
                }
            }
            catch (Exception) { }
        }

        public void OnWriteFile(string filename, ref byte[] outputReport)
        {
            try
            {
                //Console.WriteLine("OnWriteFile {0}", filename);
            }
            catch (Exception) { }
        }


        /* Config Wrappers */

        public bool ShouldEmulateController()
        {
            try
            {
                return Injector.EmulateController;
            }
            catch (Exception) { return false; }
            
        }
    }
}

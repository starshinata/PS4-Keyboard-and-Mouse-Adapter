// PS4RemotePlayInterceptor (File: Classes/Injector.cs)
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

using EasyHook;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using System.Security.Principal;

namespace PS4RemotePlayInterceptor
{
    public delegate void InterceptionDelegate(ref DualShockState state);

    public enum InjectionMode
    {
        Auto,
        Compatibility
    }

    public class Injector
    {
        // EasyHook
        private static string _channelName = null;
        private static IpcServerChannel _ipcServer;
        private static bool _noGAC = false;

        // Watchdog
        private static Watchdog m_Watchdog = new Watchdog();
        public static Watchdog Watchdog => m_Watchdog;
        public static DateTime LastPingTime { get; set; }

        // Injection
        public static InjectionMode InjectionMode = InjectionMode.Auto;
        // Emulation
        public static bool EmulateController = false;

        // Delegate
        public static InterceptionDelegate Callback { get; set; }

        public static int Inject(string processName, string dllToInject)
        {
            // Find the process
            var process = FindProcess(processName);
            if (process == null)
                throw new InterceptorException(string.Format("{0} not found in list of processes", processName));

            // Full path to our dll file
            string injectionLibrary = Path.Combine(Path.GetDirectoryName(typeof(InjectionInterface).Assembly.Location), dllToInject);

            try
            {
                bool shouldInject = false;

                if (InjectionMode == InjectionMode.Auto)
                {
                    if (_ipcServer == null)
                    {
                        // Setup remote hooking
                        _channelName = DateTime.Now.ToString();
                        _ipcServer = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName, WellKnownObjectMode.Singleton, WellKnownSidType.WorldSid);
                        shouldInject = true;
                    }
                }
                else if (InjectionMode == InjectionMode.Compatibility)
                {
                    // Setup remote hooking
                    _channelName = null;
                    _ipcServer = RemoteHooking.IpcCreateServer<InjectionInterface>(ref _channelName, WellKnownObjectMode.Singleton);
                    shouldInject = true;
                }

                // Inject dll into the process
                if (shouldInject)
                {
                    RemoteHooking.Inject(
                        process.Id, // ID of process to inject into
                        (_noGAC ? InjectionOptions.DoNotRequireStrongName : InjectionOptions.Default),
                        // if not using GAC allow assembly without strong name
                        injectionLibrary, // 32-bit version (the same because AnyCPU)
                        injectionLibrary, // 64-bit version (the same because AnyCPU)
                        _channelName
                    );
                }

                // Success
                return process.Id;
            }
            catch (Exception ex)
            {
                throw new InterceptorException(string.Format("Failed to inject to target: {0}", ex.Message), ex);
            }
        }

        public static void StopInjection()
        {
            if (_ipcServer == null)
                return;

            _ipcServer.StopListening(null);
            _ipcServer = null;
        }

        public static Process FindProcess(string processName)
        {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes)
                return process;

            return null;
        }
    }
}

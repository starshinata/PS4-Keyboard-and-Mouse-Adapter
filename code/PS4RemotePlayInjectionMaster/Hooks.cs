// PS4RemotePlayInterceptor (File: Classes/Hooks.cs)
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

using AppDomainToolkit;
using EasyHook;
using PS4RemotePlayInterceptor;
using System;
using System.Collections.Generic;

namespace PS4RemotePlayInjection
{
    /// <summary>
    /// EasyHook will look for a class implementing <see cref="EasyHook.IEntryPoint"/> during injection. This
    /// becomes the entry point within the target process after injection is complete.
    /// </summary>
    public class Hooks : EasyHook.IEntryPoint
    {

        /// <summary>
        /// Reference to the server interface
        /// </summary>
        private readonly InjectionInterface _server = null;


        /// <summary>
        /// EasyHook requires a constructor that matches <paramref name="context"/> and any additional parameters as provided
        /// in the original call to <see cref="EasyHook.RemoteHooking.Inject(int, EasyHook.InjectionOptions, string, string, object[])"/>.
        /// 
        /// Multiple constructors can exist on the same <see cref="EasyHook.IEntryPoint"/>, providing that each one has a corresponding Run method (e.g. <see cref="Run(EasyHook.RemoteHooking.IContext, string)"/>).
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public Hooks(
            EasyHook.RemoteHooking.IContext context,
            string channelName)
        {

            // Connect to server object using provided channel name
            _server = EasyHook.RemoteHooking.IpcConnectClient<InjectionInterface>(channelName);
            _server.LogDebug("InjectionInterface for " + channelName + " made");
            // If Ping fails then the Run method will be not be called
            _server.Ping();
        }


        public void DotNetHooks()
        {
            _server.LogDebug("Hooks.DotNetHooks IN");

            IList<AppDomain> appDomains = Utility.GetAppDomains();
            _server.LogDebug("Hooks.DotNetHooks appDomains " + appDomains.Count);
            foreach (AppDomain appDomain in appDomains)
            {
                _server.LogDebug("Hooks.DotNetHooks appDomain.FriendlyName " + appDomain.FriendlyName);
            }

            AppDomain remotePlayDomain = appDomains[0];

            try
            {
                var domainContext = AppDomainContext.Wrap(remotePlayDomain);
                try
                {
                    RemoteAction.Invoke(
                        domainContext.Domain,
                        () =>
                        {
                            PatcherRemoteplayToolbar.server = EasyHook.RemoteHooking.IpcConnectClient<InjectionInterface>("dotnethooks");
                            PatcherRemoteplayToolbar.DoPatching();
                        });
                }
                catch (Exception e)
                {
                    _server.LogError(e, "Error when executing remote AppDomain code");
                }

            }
            catch (Exception e)
            {
                _server.LogError(e, "Error Hooks.100");
            }
            _server.LogDebug("Hooks.DotNetHooks OUT");
        }


        /// <summary>
        /// The main entry point for our logic once injected within the target process. 
        /// This is where the hooks will be created, and a loop will be entered until host process exits.
        /// EasyHook requires a matching Run method for the constructor
        /// </summary>
        /// <param name="context">The RemoteHooking context</param>
        /// <param name="channelName">The name of the IPC channel</param>
        public void Run(
            EasyHook.RemoteHooking.IContext context,
            string channelName)
        {
            _server.LogDebug("Hooks.Run " + context + " " + channelName + " IN");

            // Injection is now complete and the server interface is connected
            _server.OnInjectionSuccess(EasyHook.RemoteHooking.GetCurrentProcessId());

            bool isX64Process = EasyHook.RemoteHooking.IsX64Process(EasyHook.RemoteHooking.GetCurrentProcessId());
            _server.LogDebug("Hooks.Run " + context + " " + channelName + " RemoteHooking Remote process is a " + (isX64Process ? "64" : "32") + "-bit process.");

            _server.LogDebug("Hooks.Run " + context + " " + channelName + " RemoteHooking IsAdministrator " + RemoteHooking.IsAdministrator);

            try
            {
                DotNetHooks();
                _server.LogDebug("Hooks.Run " + context + " " + channelName + " Dotnet hooks created.");
            }
            catch (Exception e)
            {
                _server.LogError(e, "Hooks.Run " + context + " " + channelName + " Problem creating dotnet hooks");
            }

            PatcherGamepad patcherGamepad = new PatcherGamepad(_server);
            patcherGamepad.AddGamepadHooks(channelName);

            try
            {
                // Loop until injector closes (i.e. IPC fails)
                while (true)
                {
                    System.Threading.Thread.Sleep(100);
                    _server.LogVerbose("Hooks.Run channelName " + channelName + " ping");
                    _server.Ping();
                }
            }
            catch (Exception e)
            {
                // Ping() will raise an exception if host is unreachable
                _server.LogError("Hooks.Run channelName " + channelName + " remote server is unreachable");
                _server.LogError(e.ToString());
            }

            patcherGamepad.RemoveGamepadHooks(channelName);

            _server.LogDebug("Hooks.Run " + channelName + " OUT");
        }

    }
}

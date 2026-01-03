using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace PS4RemotePlayInjection
{
    public class InjectedManager
    {
        public InjectedManager()
        {

            Console.WriteLine("Process: {0}", Process.GetCurrentProcess().ProcessName);
            Console.WriteLine("Entry assembly: {0}", Assembly.GetEntryAssembly().CodeBase);
        }
    }
}

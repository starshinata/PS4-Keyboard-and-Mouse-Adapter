using RemotePlayInjected.Injected.ControllerState;
using System.Diagnostics;

namespace RemotePlayInjected.Injected
{
    public static class UtilityData
    {
        public static Process RemotePlayProcess;
        public static int pid;
        public static bool IsCursorVisible = true;
        public static bool IsToolBarVisible = false;
        public static DualShockState DualShockState;
    }
}

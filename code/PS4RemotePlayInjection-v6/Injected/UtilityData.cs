using PS4RemotePlayInjection.Injected.ControllerState;
using System.Diagnostics;

namespace PS4RemotePlayInjection.Injected
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

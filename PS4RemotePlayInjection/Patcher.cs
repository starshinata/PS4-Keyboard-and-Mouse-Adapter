using System;
using System.Linq;
using HarmonyLib;
using PS4RemotePlayInterceptor;

namespace PS4RemotePlayInjection
{
    [Serializable]
    class Patcher
    {
        public static InjectionInterface server;
        private static bool isToolbarShown = true;

        public static void DoPatching()
        {
            Harmony harmony = new Harmony("com.example.patch");

            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            System.Reflection.Assembly dll = assemblies?.FirstOrDefault(x => x.FullName.Contains("RemotePlay"));
            Type[] types = dll?.GetTypes();

            //server.Print("Patcher.DoPatching all types ");
            foreach (Type aType in types)
            {
                server.LogDebug("Patcher.DoPatching type " + aType);
            }

            Type type = types?.FirstOrDefault(x => x.Name.Equals("StreamingToolBar"));
            server.LogDebug("Patcher.DoPatching type " + type);

            // Show toolbar patch
            try { 
                server.LogDebug("Patcher.DoPatching patching showToolBarMethod");

                System.Reflection.MethodInfo showToolBarMethod = AccessTools.Method(type, "ShowToolBar");
                System.Reflection.MethodInfo showToolBarPrefix = SymbolExtensions.GetMethodInfo(() => ShowToolBarPrefix());
                System.Reflection.MethodInfo showToolBarPostfix = SymbolExtensions.GetMethodInfo(() => ShowToolBarPostfix());
                // in general, add null checks here (new HarmonyMethod() does it for you too)

                harmony.Patch(showToolBarMethod, new HarmonyMethod(showToolBarPrefix), new HarmonyMethod(showToolBarPostfix));
                server.LogDebug("Patcher.DoPatching patched showToolBarMethod");
            } catch (Exception e) {
                server.LogError(e, "Patcher.DoPatching patching failed showToolBarMethod");
            }

            // Hide toolbar patch
            try { 
                server.LogDebug("Patcher.DoPatching patching hideToolBarMethod");
                
                System.Reflection.MethodInfo hideToolBarMethod = AccessTools.Method(type, "HideToolBar");
                System.Reflection.MethodInfo hideToolBarPrefix = SymbolExtensions.GetMethodInfo(() => HideToolBarPrefix());
                System.Reflection.MethodInfo hideToolBarPostfix = SymbolExtensions.GetMethodInfo(() => HideToolBarPostfix());

                harmony.Patch(hideToolBarMethod, new HarmonyMethod(hideToolBarPrefix), new HarmonyMethod(hideToolBarPostfix));
                server.LogDebug("Patcher.DoPatching patched hideToolBarMethod");
            } catch (Exception e) {
                server.LogError(e, "Patcher.DoPatching patching failed hideToolBarMethod");
            }
        }

        public static bool ShowToolBarPrefix()
        {
            bool showToolbar = server.ShouldShowToolbar();

            if (showToolbar)
                isToolbarShown = true;

            return showToolbar;
        }

        public static void ShowToolBarPostfix()
        {
            // ...
        }

        public static bool HideToolBarPrefix()
        {
            bool dontHideHud = server.ShouldShowToolbar();

            if (dontHideHud || (!dontHideHud && isToolbarShown))
            {
                isToolbarShown = false;
                return true;
            }
            return false;
        }

        public static void HideToolBarPostfix()
        {
            // ...
        }
    }
}

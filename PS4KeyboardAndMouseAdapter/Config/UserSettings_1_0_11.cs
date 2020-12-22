using System;
using System.Collections.Generic;
using System.Reflection;
using Serilog;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public class UserSettings_1_0_11
    {

        private static ILogger staticLogger = Log.ForContext(typeof(UserSettings_1_0_11));

        ////////////////////////////////////////////////////////////////////////////

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; set; } = new Dictionary<VirtualKey, Keyboard.Key>();

        public int AnalogStickLowerRange { get; set; } = 40;
        public int AnalogStickUpperRange { get; set; } = 95;

        public bool MouseControlsL3 { get; set; } = false;
        public bool MouseControlsR3 { get; set; } = false;

        public double MouseDistanceLowerRange { get; set; } = 5;
        public double MouseDistanceUpperRange { get; set; } = VideoMode.DesktopMode.Width / 20f;
        public double MouseMaxDistance { get; set; } = VideoMode.DesktopMode.Width / 2f;

        public int MousePollingRate { get; set; }

        public double MouseXAxisSensitivityMax { get; set; } = 100;
        public double MouseXAxisSensitivityModifier { get; set; } = 2;
        public double MouseYAxisSensitivityMax { get; set; } = 100;
        public double MouseYAxisSensitivityModifier { get; set; } = 1;

        public double XYRatio { get; set; } = 0.6;

        //////////////////////////////////////////////////////////////////////

        private static void AddManualMouseMapping(UserSettings newSettings, VirtualKey vk, MouseButton mouseButton)
        {
            if (newSettings.Mappings[vk] == null)
            {
                newSettings.Mappings[vk] = new PhysicalKeyGroup();
            }

            PhysicalKey pk = new PhysicalKey();
            pk.MouseValue = mouseButton;

            newSettings.Mappings[vk].PhysicalKeys.Add(pk);
        }

        public static PhysicalKeyGroup GetPhysicalKeyGroup(PhysicalKey pk)
        {
            PhysicalKeyGroup pkg = new PhysicalKeyGroup();
            pkg.PhysicalKeys.Add(pk);
            return pkg;
        }

        public static UserSettings ImportValues(UserSettings_1_0_11 legacySettings)
        {
            Console.WriteLine("UserSettings_1_0_11.ImportValues()");
            staticLogger.Information("UserSettings_1_0_11.ImportValues()");

            UserSettings newSettings = UserSettings.GetInstance();

            newSettings.AnalogStickLowerRange = legacySettings.AnalogStickLowerRange;
            newSettings.AnalogStickUpperRange = legacySettings.AnalogStickUpperRange;

            newSettings.MouseControlsL3 = legacySettings.MouseControlsL3;
            newSettings.MouseControlsR3 = legacySettings.MouseControlsR3;

            newSettings.MouseDistanceLowerRange = legacySettings.MouseDistanceLowerRange;
            newSettings.MouseDistanceUpperRange = legacySettings.MouseDistanceUpperRange;
            newSettings.MouseMaxDistance = legacySettings.MouseMaxDistance;

            newSettings.MousePollingRate = legacySettings.MousePollingRate;

            newSettings.MouseXAxisSensitivityAimModifier = legacySettings.MouseXAxisSensitivityModifier;
            newSettings.MouseXAxisSensitivityLookModifier = legacySettings.MouseXAxisSensitivityModifier;
            newSettings.MouseXAxisSensitivityMax = legacySettings.MouseXAxisSensitivityMax;

            newSettings.MouseYAxisSensitivityAimModifier = legacySettings.MouseYAxisSensitivityModifier;
            newSettings.MouseYAxisSensitivityLookModifier = legacySettings.MouseYAxisSensitivityModifier;
            newSettings.MouseYAxisSensitivityMax = legacySettings.MouseYAxisSensitivityMax;

            newSettings.XYRatio = legacySettings.XYRatio;


            foreach (VirtualKey key in legacySettings.Mappings.Keys)
            {
                PhysicalKey pk = new PhysicalKey();
                pk.KeyboardValue = legacySettings.Mappings[key];

                newSettings.Mappings[key] = GetPhysicalKeyGroup(pk);
            }

            // Now readd the mouse bindings that didnt exist as config in 1.0.11
            AddManualMouseMapping(newSettings, VirtualKey.L2, MouseButton.Right);
            AddManualMouseMapping(newSettings, VirtualKey.R2, MouseButton.Left);

            newSettings.Version_1_0_12_OrGreater = true;

            return newSettings;
        }

        public static void Print(UserSettings_1_0_11 settings)
        {

            Console.WriteLine("UserSettings_1_0_11.Print()");

            Console.WriteLine("mappings");
            foreach (VirtualKey key in settings.Mappings.Keys)
            {
                Console.WriteLine("print Mappings:{VirtKey:" + key + ", keyboardValue: " + settings.Mappings[key] + "}");
            }


            Console.WriteLine("values");

            Type t = settings.GetType();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo prop in properties)
            {
                if (prop.Name != "Mappings")
                {
                    MethodInfo getter = prop.GetGetMethod();
                    if (getter != null)
                    {
                        var value = getter.Invoke(settings, new object[] { });

                        Console.WriteLine("print " + prop + ":" + value);
                    }
                }
            }
        }
    }
}
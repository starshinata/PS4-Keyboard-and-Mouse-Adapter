using Newtonsoft.Json;
using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using Serilog;
using SFML.Window;
using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Config
{
    public class UserSettingsV1
    {
        ////////////////////////////////////////////////////////////////////////////
        /// Static props
        ////////////////////////////////////////////////////////////////////////////
        private static readonly ILogger StaticLogger = Log.ForContext(typeof(UserSettingsV1));

        ////////////////////////////////////////////////////////////////////////////
        /// Instance props
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

        public double XYRatio { get; set; } = 1;

        ////////////////////////////////////////////////////////////////////////////
        /// Static Methods
        ////////////////////////////////////////////////////////////////////////////


        private static void AddMapping(UserSettingsV3 newSettings, VirtualKey vk, PhysicalKey pk)
        {
            Mapping mapping = new Mapping();
            mapping.uid = UserSettingsContainer.GetNextMappingUid();
            mapping.PhysicalKeys.Add(pk);
            mapping.VirtualKeys.Add(vk);

            newSettings.Mappings.Add(mapping);
        }

        private static void AddMouseMapping(UserSettingsV3 newSettings, VirtualKey vk, MouseButton mouseButton)
        {
            PhysicalKey pk = new PhysicalKey();
            pk.MouseValue = mouseButton;
            AddMapping(newSettings, vk, pk);
        }

        public static UserSettingsV3 ImportValues(string json)
        {
            StaticLogger.Information("UserSettingsV1.ImportValues()");

            UserSettingsV1 legacySettings = JsonConvert.DeserializeObject<UserSettingsV1>(json);
            UserSettingsV3 newSettings = new UserSettingsV3();

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


            foreach (VirtualKey vk in legacySettings.Mappings.Keys)
            {
                PhysicalKey pk = new PhysicalKey();
                pk.KeyboardValue = legacySettings.Mappings[vk];

                AddMapping(newSettings, vk, pk);
            }


            // Now add the mouse bindings that didnt exist as config in V1
            AddMouseMapping(newSettings, VirtualKey.L2, MouseButton.Right);
            AddMouseMapping(newSettings, VirtualKey.R2, MouseButton.Left);

            newSettings.RefreshOptimisations();

            return newSettings;
        }

    }
}

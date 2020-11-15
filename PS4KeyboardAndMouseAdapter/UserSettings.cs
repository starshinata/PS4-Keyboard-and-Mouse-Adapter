using System.Collections.Generic;
using SFML.Window;

namespace PS4KeyboardAndMouseAdapter
{

    public class UserSettings
    {

        public Dictionary<VirtualKey, Keyboard.Key> Mappings { get; set; }

        //////////////////////////////////////////////////////////////////////

        public int AnalogStickLowerRange { get; set; } = 40;
        public int AnalogStickUpperRange { get; set; } = 95;

        public double MouseDistanceLowerRange { get; set; } = 5;
        public double MouseDistanceUpperRange { get; set; } = VideoMode.DesktopMode.Width / 20f;
        public double MouseMaxDistance => VideoMode.DesktopMode.Width / 2f;

        public bool MouseControlsL3 { get; set; } = false;
        public bool MouseControlsR3 { get; set; } = false;


        private int _MousePollingRate;

        // DualShock 4 polling rate
        // 60  Hz Bluetooth
        // 250 Hz Wired
        // so it doesnt make sense for mouse polling rate to be above 250
        public int MousePollingRate
        {
            get
            {
                return _MousePollingRate;
            }
            set
            {
                if (value < 10)
                {
                    _MousePollingRate = 10;
                }
                else if (value > 250)
                {

                    _MousePollingRate = 250;
                }
                else
                {
                    _MousePollingRate = value;
                }
            }
        }


        public double MouseXAxisSensitivityMax { get; set; } = 100;
        public double MouseXAxisSensitivityModifier { get; set; } = 2;
        public double MouseYAxisSensitivityMax { get; set; } = 100;
        public double MouseYAxisSensitivityModifier { get; set; } = 1;


        //TODO do we still need this ?
        public double XYRatio { get; set; } = 0.6;

        public UserSettings()
        {
            MousePollingRate = 60;
        }
    }

}

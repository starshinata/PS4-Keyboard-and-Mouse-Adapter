using SFML.Window;
using static SFML.Window.Keyboard;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public class PhysicalKey
    {
        public Key KeyboardValue = Key.Unknown;
        public MouseButton MouseValue = MouseButton.Unknown;

        public override string ToString()
        {
            if (MouseValue != MouseButton.Unknown)
            {
                return MouseValue.ToString();
            }


            if (KeyboardValue != Key.Unknown)
            {
                return KeyboardValue.ToString();
            }



            return "";
        }
    }
}

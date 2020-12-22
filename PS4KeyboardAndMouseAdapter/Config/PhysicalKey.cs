using static SFML.Window.Keyboard;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public class PhysicalKey
    {
        public Key KeyboardValue = Key.Unknown;
        public MouseButton MouseValue = MouseButton.Unknown;

        public string KeyboardButtonToString(Key key)
        {

            var displayValue = key.ToString();

            if (displayValue.Contains("Num"))
                displayValue = displayValue.Replace("Num", "");

            if (displayValue.Equals("Left"))
                displayValue = "\u21e6 Left";

            if (displayValue.Equals("Up"))
                displayValue = "\u21e7 Up";

            if (displayValue.Equals("Right"))
                displayValue = "\u21e8 Right";

            if (displayValue.Equals("Down"))
                displayValue = "\u21e9 Down";

            return displayValue;
        }

        public string MouseButtonToString(MouseButton key)
        {
            return "Mouse " + key.ToString();
        }

        public override string ToString()
        {
            if (MouseValue != MouseButton.Unknown)
            {
                return MouseButtonToString(  MouseValue);
            }

            if (KeyboardValue != Key.Unknown)
            {
                return KeyboardButtonToString(KeyboardValue);
            }

            return "";
        }
    }
}

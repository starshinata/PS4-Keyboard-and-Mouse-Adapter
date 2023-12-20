using static SFML.Window.Keyboard;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Mappings
{
    public class PhysicalKey
    {
        public ExtraButtons ExtraValue = ExtraButtons.Unknown;
        public Key KeyboardValue = Key.Unknown;
        public MouseButton MouseValue = MouseButton.Unknown;

        public override bool Equals(object obj)
        {
            return obj is PhysicalKey key &&
                   ExtraValue == key.ExtraValue &&
                   KeyboardValue == key.KeyboardValue &&
                   MouseValue == key.MouseValue;
        }

        public string ExtraButtonToString(ExtraButtons key)
        {
            return key.ToString();
        }

        public override int GetHashCode()
        {
            int hashCode = -1994028664;
            hashCode = hashCode * -1521134295 + ExtraValue.GetHashCode();
            hashCode = hashCode * -1521134295 + KeyboardValue.GetHashCode();
            hashCode = hashCode * -1521134295 + MouseValue.GetHashCode();
            return hashCode;
        }

        public string KeyboardButtonToString(Key key)
        {

            string displayValue = key.ToString();

            if (displayValue.Contains("Num"))
                displayValue = displayValue.Replace("Num", "");

            if (key == Key.F1)
                displayValue = "F1";

            if (key == Key.F2)
                displayValue = "F2";

            if (key == Key.F3)
                displayValue = "F3";

            if (key == Key.F4)
                displayValue = "F4";

            if (key == Key.F5)
                displayValue = "F5";

            if (key == Key.F6)
                displayValue = "F6";

            if (key == Key.F7)
                displayValue = "F8";

            if (key == Key.F8)
                displayValue = "F8";

            if (key == Key.F9)
                displayValue = "F9";

            if (key == Key.F10)
                displayValue = "F10";


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
            if (ExtraValue != ExtraButtons.Unknown)
            {
                return ExtraButtonToString(ExtraValue);
            }

            if (KeyboardValue != Key.Unknown)
            {
                return KeyboardButtonToString(KeyboardValue);
            }

            if (MouseValue != MouseButton.Unknown)
            {
                return MouseButtonToString(MouseValue);
            }

            return "";
        }

    }
}

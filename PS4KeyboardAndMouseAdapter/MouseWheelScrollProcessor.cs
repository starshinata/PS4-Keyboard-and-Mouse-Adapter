using PS4KeyboardAndMouseAdapter.Config;

namespace PS4KeyboardAndMouseAdapter
{
    // The two methods in this class are intentionally similar
    // But notice the different input parameter type
    // I could not easily cast either of these into the other
    // And I can settle for a little duplication here
    class MouseWheelScrollProcessor
    {

        public static ExtraButtons GetScrollAction(System.Windows.Forms.MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Delta > 0)
                {
                    return ExtraButtons.MouseWheelUp;
                }

                if (e.Delta < 0)
                {
                    return ExtraButtons.MouseWheelDown;
                }
            }

            return ExtraButtons.Unknown;
        }

        public static ExtraButtons GetScrollAction(System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e != null)
            {
                if (e.Delta > 0)
                {
                    return ExtraButtons.MouseWheelUp;
                }

                if (e.Delta < 0)
                {
                    return ExtraButtons.MouseWheelDown;
                }
            }
            return ExtraButtons.Unknown;
        }

    }
}

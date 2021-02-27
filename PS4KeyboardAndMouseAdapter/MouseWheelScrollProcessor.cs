using PS4KeyboardAndMouseAdapter.Config;

namespace PS4KeyboardAndMouseAdapter
{
    // the two methods in this class are intentionally similar
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

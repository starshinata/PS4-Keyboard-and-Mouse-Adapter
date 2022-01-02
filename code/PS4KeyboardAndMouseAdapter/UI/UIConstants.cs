using System.Windows;
using System.Windows.Media;

namespace PS4KeyboardAndMouseAdapter.UI
{
    public class UIConstants
    {
        public const double LOW_VISIBILITY = 0.1;

        public static readonly Brush TEXTBOX_COLOUR_GREEN = new SolidColorBrush(Colors.LimeGreen);
        public static readonly Brush TEXTBOX_COLOUR_RED = new SolidColorBrush(Colors.Red);

        public const Visibility VISIBILITY_COLLAPSED = Visibility.Collapsed;
        // generally you want Collapsed instead of hidden as the space wont be taken
        public const Visibility VISIBILITY_HIDDEN = Visibility.Hidden;
        public const Visibility VISIBILITY_VISIBLE = Visibility.Visible;

    }
}

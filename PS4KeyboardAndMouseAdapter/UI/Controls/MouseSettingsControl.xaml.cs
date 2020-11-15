using System.Windows;
using System.Windows.Controls;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    /// <summary>
    /// Interaction logic for MouseSettingsControl.xaml
    /// </summary>
    public partial class MouseSettingsControl : UserControl
    {

        public static readonly DependencyProperty AnalogStickLowerRangeProperty = DependencyProperty.Register(
       "AnalogStickLowerRange", typeof(int), typeof(MouseSettingsControl), new PropertyMetadata(default(int)));

        public int AnalogStickLowerRange
        {
            get => (int)GetValue(AnalogStickLowerRangeProperty);
            set => SetValue(AnalogStickLowerRangeProperty, value);
        }


        /////////////////////////

        public static readonly DependencyProperty AnalogStickUpperRangeProperty = DependencyProperty.Register(
            "AnalogStickUpperRange", typeof(int), typeof(MouseSettingsControl), new PropertyMetadata(default(int)));

        public int AnalogStickUpperRange
        {
            get => (int)GetValue(AnalogStickUpperRangeProperty);
            set => SetValue(AnalogStickUpperRangeProperty, value);
        }

        /////////////////////////

        public static readonly DependencyProperty MouseDistanceMinRangeProperty = DependencyProperty.Register(
            "MouseDistanceMinRange", typeof(double), typeof(MouseSettingsControl), new PropertyMetadata(default(double)));

        public double MouseDistanceMinRange
        {
            get => (double)GetValue(MouseDistanceMinRangeProperty);
            set => SetValue(MouseDistanceMinRangeProperty, value);
        }

        /////////////////////////

        public static readonly DependencyProperty MouseDistanceMaxRangeProperty = DependencyProperty.Register(
            "MouseDistanceMaxRange", typeof(double), typeof(MouseSettingsControl), new PropertyMetadata(default(double)));

        public double MouseDistanceMaxRange
        {
            get => (double)GetValue(MouseDistanceMaxRangeProperty);
            set => SetValue(MouseDistanceMaxRangeProperty, value);
        }

        /////////////////////////

        public static readonly DependencyProperty MaxMouseDistanceProperty = DependencyProperty.Register(
            "MaxMouseDistance", typeof(double), typeof(MouseSettingsControl), new PropertyMetadata(default(double)));

        public double MaxMouseDistance
        {
            get => (double)GetValue(MaxMouseDistanceProperty);
            set => SetValue(MaxMouseDistanceProperty, value);
        }

        /////////////////////////

        public static readonly DependencyProperty HorizontalVerticalRatioProperty = DependencyProperty.Register(
         "HorizontalVerticalRatio", typeof(double), typeof(MouseSettingsControl), new PropertyMetadata(default(double)));

        public double HorizontalVerticalRatio
        {
            get => (double)GetValue(HorizontalVerticalRatioProperty);
            set => SetValue(HorizontalVerticalRatioProperty, value);
        }

        /////////////////////////

        public MouseSettingsControl()
        {
            InitializeComponent();
        }
    }
}

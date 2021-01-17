using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PS4KeyboardAndMouseAdapter.UI.Controls
{
    public partial class RangeSlider : UserControl
    {
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d, null, LowerValueCoerceValueCallback));

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d, null, UpperValueCoerceValueCallback));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d));

        public static readonly DependencyProperty IsSnapToTickEnabledProperty =
            DependencyProperty.Register("IsSnapToTickEnabled", typeof(bool), typeof(RangeSlider), new UIPropertyMetadata(false));

        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register("TickFrequency", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0.1d));

        public static readonly DependencyProperty TickPlacementProperty =
            DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(RangeSlider), new UIPropertyMetadata(TickPlacement.None));

        public static readonly DependencyProperty TicksProperty =
            DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(RangeSlider), new UIPropertyMetadata(null));

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(RangeSlider), new PropertyMetadata(default(string)));

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double LowerValue
        {
            get => (double)GetValue(LowerValueProperty);
            set => SetValue(LowerValueProperty, value);
        }

        public double UpperValue
        {
            get => (double)GetValue(UpperValueProperty);
            set => SetValue(UpperValueProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public bool IsSnapToTickEnabled
        {
            get => (bool)GetValue(IsSnapToTickEnabledProperty);
            set => SetValue(IsSnapToTickEnabledProperty, value);
        }

        public double TickFrequency
        {
            get => (double)GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }

        public TickPlacement TickPlacement
        {
            get => (TickPlacement)GetValue(TickPlacementProperty);
            set => SetValue(TickPlacementProperty, value);
        }

        public DoubleCollection Ticks
        {
            get => (DoubleCollection)GetValue(TicksProperty);
            set => SetValue(TicksProperty, value);
        }

        private static object LowerValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            RangeSlider targetSlider = (RangeSlider)target;
            double value = (double)valueObject;

            return Math.Abs(targetSlider.UpperValue) < 0.0001 ? value : Math.Min(value, targetSlider.UpperValue);
        }

        private static object UpperValueCoerceValueCallback(DependencyObject target, object valueObject)
        {
            RangeSlider targetSlider = (RangeSlider)target;
            double value = (double)valueObject;

            return Math.Max(value, targetSlider.LowerValue);
        }

        public RangeSlider()
        {
            InitializeComponent();
        }

        private void RangeSlider_OnLoaded(object sender, RoutedEventArgs e)
        {
            var slider = sender as RangeSlider;
            Slider topSlider = slider.upperSlider;

            var c = UITools.FindVisualChildren<RepeatButton>(topSlider);

            foreach (RepeatButton repeatButton in c)
            {
                repeatButton.Visibility = Visibility.Collapsed;
            }

            foreach (Border border in UITools.FindVisualChildren<Border>(topSlider))
            {
                if (border.Name.Equals("TrackBackground"))
                    border.Visibility = Visibility.Collapsed;
            }
        }
    }
}

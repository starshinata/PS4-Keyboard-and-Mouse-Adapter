using System;
using System.Windows;
using System.Windows.Controls;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls
{

    public partial class SliderV : UserControl
    {

        public event EventHandler ValueChanged = delegate { };

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(SliderV), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        //////////////////////////////////////////////////////

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum", typeof(float), typeof(SliderV), new PropertyMetadata(default(float)));

        public float Minimum
        {
            get => (float) GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        //////////////////////////////////////////////////////

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum", typeof(int), typeof(SliderV), new PropertyMetadata(default(int)));

        public int Maximum
        {
            get => (int) GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        //////////////////////////////////////////////////////

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(double), typeof(SliderV), new PropertyMetadata(default(double)));

        public double Value
        {
            get => (double) GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        
        //////////////////////////////////////////////////////

        public static readonly DependencyProperty TickFrequencyProperty = DependencyProperty.Register(
            "TickFrequency", typeof(double), typeof(SliderV), new PropertyMetadata(default(double)));

        public double TickFrequency
        {
            get => (double) GetValue(TickFrequencyProperty);
            set => SetValue(TickFrequencyProperty, value);
        }

        public SliderV()
        {
            InitializeComponent();
        }

        private void Handler_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ValueChanged(this, EventArgs.Empty);
        }
    }
}

using Pizza.KeyboardAndMouseAdapter.Backend;
using System.Windows;
using System.Windows.Controls;


namespace Pizza.KeyboardAndMouseAdapter.UI.Controls
{
    /// <summary>
    /// Interaction logic for AnalogStickVisualizer.xaml
    /// </summary>
    public partial class AnalogStickVisualizer : UserControl
    {
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(int), typeof(AnalogStickVisualizer), new PropertyMetadata(default(int)));

        public int Diameter
        {
            get => (int) GetValue(DiameterProperty);
            set => SetValue(DiameterProperty, value);
        }

        public static readonly DependencyProperty analogXProperty = DependencyProperty.Register(
            "analogX", typeof(int), typeof(AnalogStickVisualizer), new PropertyMetadata(default(int), OnAnalogXChanged));

        private static void OnAnalogXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public int analogX
        {
            get => (int) GetValue(analogXProperty);
            set => SetValue(analogXProperty, value);
        }

        public static readonly DependencyProperty analogYProperty = DependencyProperty.Register(
            "analogY", typeof(int), typeof(AnalogStickVisualizer), new PropertyMetadata(default(int), OnAnalogYChanged));

        private static void OnAnalogYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateCoords(d as AnalogStickVisualizer);
        }

        public int analogY
        {
            get => (int) GetValue(analogYProperty);
            set => SetValue(analogYProperty, value);
        }

        public AnalogStickVisualizer()
        {
            InitializeComponent();
        }

        private static void UpdateCoords( AnalogStickVisualizer v)
        {
            var circleDotRatio = 3;
            int radius = v.Diameter / 2;
            v.dot.Width = (double)radius / circleDotRatio;
            v.dot.Height = (double)radius / circleDotRatio;

            var dotLeftmostX = Canvas.GetLeft(v.circle) - v.dot.Width / 2;
            var dotTopmostY = Canvas.GetTop(v.circle) - v.dot.Height / 2;
            var dotOffsetX = AngleUtility.map(v.analogX, 0, 255, 0, v.Diameter);
            var dotOffsetY = AngleUtility.map(v.analogY, 0, 255, 0, v.Diameter);
            Canvas.SetLeft(v.dot, dotLeftmostX + dotOffsetX);
            Canvas.SetTop(v.dot, dotTopmostY + dotOffsetY);
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCoords(sender as AnalogStickVisualizer);
        }
    }
}

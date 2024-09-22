using System.Windows;
using System.Windows.Controls;
using PS4RemotePlayInjection;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController
{
    /// <summary>
    /// Interaction logic for AnalogStickVisualizer.xaml
    /// </summary>
    public partial class AnalogStickMouseInput : UserControl
    {
        public static readonly DependencyProperty DiameterProperty = DependencyProperty.Register(
            "Diameter", typeof(int), typeof(AnalogStickVisualizer), new PropertyMetadata(default(int)));

        public AnalogStickMouseInput()
        {
            InitializeComponent();
        }

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

 

        private static void UpdateCoords( AnalogStickVisualizer v)
        {
            var circleDotRatio = 3;
            int radius = v.Diameter / 2;
            v.dot.Width = (double)radius / circleDotRatio;
            v.dot.Height = (double)radius / circleDotRatio;

            var dotLeftmostX = Canvas.GetLeft(v.circle) - v.dot.Width / 2;
            var dotTopmostY = Canvas.GetTop(v.circle) - v.dot.Height / 2;
            var dotOffsetX = Utility.map(v.analogX, 0, 255, 0, v.Diameter);
            var dotOffsetY = Utility.map(v.analogY, 0, 255, 0, v.Diameter);
            Canvas.SetLeft(v.dot, dotLeftmostX + dotOffsetX);
            Canvas.SetTop(v.dot, dotTopmostY + dotOffsetY);
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCoords(sender as AnalogStickVisualizer);
        }
    }
}

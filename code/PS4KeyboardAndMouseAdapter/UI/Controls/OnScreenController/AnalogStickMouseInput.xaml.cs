using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController
{
    /// <summary>
    /// Interaction logic for AnalogStickVisualizer.xaml
    /// </summary>
    public partial class AnalogStickMouseInput : UserControl
    {

        public AnalogStickMouseInput()
        {
            InitializeComponent();
        }

        public void RepaintStickCenter()
        {
            if (ActualHeight > 10 && ActualWidth > 10)
            {
                double diameter = Math.Min(ActualHeight, ActualWidth);
                double shapeCircleOffset = (ActualWidth - diameter) / 2.0;
                Canvas.SetLeft(shapeCircle, shapeCircleOffset);

                shapeCircle.Height = diameter;
                shapeCircle.Width = diameter;

                shapeCenter.Height = 10;
                shapeCenter.Width = 10;

                double shapeCenterTopOffset =  (diameter / 2) - (shapeCenter.Width / 2);
                double shapeCenterLeftOffset = shapeCircleOffset + shapeCenterTopOffset;
                Canvas.SetLeft(shapeCenter, shapeCenterLeftOffset);
                Canvas.SetTop(shapeCenter, shapeCenterTopOffset);
            }
        }

        //TODO
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("Canvas_MouseMove");
        }

        //TODO mouse drag

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("Canvas_MouseDown");
            Point p = Mouse.GetPosition(canvas);
            Console.WriteLine(p);
        }
    }
}

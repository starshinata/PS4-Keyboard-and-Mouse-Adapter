using Pizza.KeyboardAndMouseAdapter.Backend.ControllerState;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pizza.KeyboardAndMouseAdapter.UI.Controls.OnScreenController
{
    public partial class AnalogStickMouseInput : UserControl
    {

        private Point centre;
        private bool clicked = false;
        private double radius;
        private string label;


        // If not null, you will have a Point
        // in Point you have two Coordinates
        // Each coordinates is a range between -1 and 1
        // 
        // think of
        // -1 as 100% Left
        // 1 as 100% Right
        // -1 as 100% Up
        // 1 as 100% Down
        public NullablePoint pointAsPercentage;


        public AnalogStickMouseInput()
        {
            InitializeComponent();
        }

        public void RepaintStickCenter()
        {
            if (ActualHeight > 10 && ActualWidth > 10)
            {

                // We are setting circle horizontal alignment to centred
                // we are NOT setting circle vertical alignment, so top will always be ZERO

                double diameter = Math.Min(ActualHeight, ActualWidth);
                radius = diameter / 2;

                // shape CIRCLE START

                double shapeCircle_offsetLeft = (ActualWidth - diameter) / 2.0;
                Canvas.SetLeft(shapeCircle, shapeCircle_offsetLeft);

                shapeCircle.Height = diameter;
                shapeCircle.Width = diameter;
                // shape CIRCLE END


                centre = new Point(shapeCircle_offsetLeft + radius, radius);


                // shape CENTRE START
                shapeCenter.Height = 10;
                shapeCenter.Width = 10;

                double shapeCenter_offsetTop = (radius) - (shapeCenter.Width / 2);
                double shapeCenter_offsetLeft = shapeCircle_offsetLeft + shapeCenter_offsetTop;

                Canvas.SetLeft(shapeCenter, shapeCenter_offsetLeft);
                Canvas.SetTop(shapeCenter, shapeCenter_offsetTop);
                // shape CENTRE END
            }
        }

        public void SetLabel(String s)
        {
            label = s;
        }

        // references
        // is a point in a circle - https://youtu.be/S6BHQMk8C_A?si=yOPoWzh0FvsieTh4
        // having watched this video i now feel dumb for drawing a blank on how to detect if a point in a circle
        // https://stackoverflow.com/questions/11555355/calculating-the-distance-between-2-points
        private bool isInCircle(Point mousePoint)
        {
            // when debugging
            // if the canvas background isnt painted
            // then you will almost always see TRUE as the result
            // if the canvas background is painted
            // then you will see a sensible amount of TRUE and FALSE

            //Console.WriteLine("isInCircle" + label + " centre " + centre);
            //Console.WriteLine("isInCircle" + label + " radius " + radius);
            double distance = Point.Subtract(mousePoint, centre).Length;
            //Console.WriteLine("isInCircle" + label + " distance " + distance);
            return distance < radius;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = Mouse.GetPosition(canvas);
            if (clicked && isInCircle(p))
            {
                //Console.WriteLine("Canvas_MouseMove " + label + " current point " + p);

                double centeredX = p.X - centre.X;
                double centeredY = p.Y - centre.Y;

                pointAsPercentage = new NullablePoint(centeredX / radius, centeredY / radius);
                //Console.WriteLine("Canvas_MouseMove " + label + " scaled point " + pointAsPercentage.X + ", " + pointAsPercentage.Y);

            }

            //Console.WriteLine("Canvas_MouseMove " + label + " current point " + p);
            //Console.WriteLine("isInCircle " + isInCircle(p));
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("Canvas_MouseDown " + label);
            //Point p = Mouse.GetPosition(canvas);
            //Console.WriteLine(p);
            //Console.WriteLine("isInCircle " + isInCircle(p) + " " + p);
            clicked = true;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            clicked = false;
            pointAsPercentage = null;
        }
    }
}

using Pizza.KeyboardAndMouseAdapter.Backend.Config;
using Pizza.KeyboardAndMouseAdapter.Backend.ControllerState;
using Serilog;
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
        private InstanceSettings InstanceSettings { get; set; } = InstanceSettings.GetInstance();
        private string label;
        private double radius;

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

        public void SetLabel(string s)
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

            //Log.Information("AnalogStickMouseInput isInCircle" + label + " centre " + centre);
            //Log.Information("AnalogStickMouseInput isInCircle" + label + " radius " + radius);
            double distance = Point.Subtract(mousePoint, centre).Length;
            //Log.Information("AnalogStickMouseInput isInCircle" + label + " distance " + distance);
            return distance < radius;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Log.Information("AnalogStickMouseInput Canvas_MouseDown label " + label);
            Point p = Mouse.GetPosition(canvas);
            Log.Information("AnalogStickMouseInput Canvas_MouseDown p " + p);
            Log.Information("AnalogStickMouseInput Canvas_MouseDown isInCircle " + isInCircle(p) + " " + p);
            clicked = true;
            UpdateStickWithValue();
        }

        private void Canvas_MouseLeave(object sender, EventArgs e)
        {
            clicked = false;
            UpdateStickReset();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateStickWithValue();
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            clicked = false;
            UpdateStickReset();
        }

        private void UpdateStickReset()
        {
            InstanceSettings.GetInstance().SetStick(label, null);
        }

        private void UpdateStickWithValue()
        {
            Point p = Mouse.GetPosition(canvas);
            bool inCircle = isInCircle(p);

            if (inCircle)
            {

                if ((InstanceSettings.OnscreenSticksClickRequired && clicked) ||
                    !InstanceSettings.OnscreenSticksClickRequired)
                {
                    Log.Information("AnalogStickMouseInput UpdateStickWithValue " + label + " current point " + p);

                    double centeredX = p.X - centre.X;
                    double centeredY = p.Y - centre.Y;

                    // by dividing it by radius we are basically making it a percentage of radius,
                    // negative means left or up
                    // positive means right or down 
                    NullablePoint point = new NullablePoint(centeredX / radius, centeredY / radius);
                    InstanceSettings.GetInstance().SetStick(label, point);

                    Log.Information("AnalogStickMouseInput UpdateStickWithValue " + label + " saved point " + point.X + ", " + point.Y);

                }

            }
            else
            {
                UpdateStickReset();
            }

            Log.Information("AnalogStickMouseInput UpdateStickWithValue 142 " + label + " current point " + p);
            Log.Information("AnalogStickMouseInput UpdateStickWithValue 143 isInCircle " + isInCircle(p));
        }
    }
}

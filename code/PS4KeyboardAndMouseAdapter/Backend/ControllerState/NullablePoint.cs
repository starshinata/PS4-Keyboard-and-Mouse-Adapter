
namespace Pizza.KeyboardAndMouseAdapter.Backend.ControllerState
{
    // Ideally we could just use POINT,
    // but Point is a struct not a class, and thus isnt Nullable
    public class NullablePoint
    {
        public double X = 0;
        public double Y = 0;

        public NullablePoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}

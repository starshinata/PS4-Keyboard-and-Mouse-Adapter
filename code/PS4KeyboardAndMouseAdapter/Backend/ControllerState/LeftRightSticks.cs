
namespace Pizza.KeyboardAndMouseAdapter.Backend.ControllerState
{
    public class LeftRightSticks
    {
        public NullablePoint Left;
        public NullablePoint Right;

        public LeftRightSticks(NullablePoint left, NullablePoint right)
        {
            Left = left;
            Right = right;
        }
    }
}

namespace RemotePlayInjected.Injected.ControllerState
{
    // References
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Device.cs
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Sixaxis.cs
    // https://github.com/Jays2Kings/DS4Windows/blob/jay/DS4Windows/DS4Library/DS4Touchpad.cs
    // http://www.psdevwiki.com/ps4/DS4-USB

    public class Touch
    {
        public byte TouchID { get; set; }
        public bool IsTouched { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        /* Constructors */
        public Touch()
        {
            TouchID = 0;
            IsTouched = false;
            X = 0;
            Y = 0;
        }
        public Touch(byte touchID, bool isTouched, int x, int y)
        {
            TouchID = touchID;
            IsTouched = isTouched;
            X = x;
            Y = y;
        }
        public Touch(Touch touch)
        {
            TouchID = touch.TouchID;
            IsTouched = touch.IsTouched;
            X = touch.X;
            Y = touch.Y;
        }

        public Touch Clone()
        {
            return new Touch(this);
        }
    }

}

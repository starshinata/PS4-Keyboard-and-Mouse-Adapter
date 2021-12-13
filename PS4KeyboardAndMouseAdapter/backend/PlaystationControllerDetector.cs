using Pizza.backend;
using Serilog;

namespace PS4KeyboardAndMouseAdapter.backend
{
    class PlaystationControllerDetector
    {

        private static readonly ILogger StaticLogger = Log.ForContext(typeof(PlaystationControllerDetector));

        public static void DetectControllers()
        {
            // refresh SFML's joystick list, else it be reported that there are 0 controllers/joysticks connected
            SFML.Window.Joystick.Update();

            int connectedCount = 0;
            for (uint i = 0; i < SFML.Window.Joystick.Count; i++)
            {
                if (SFML.Window.Joystick.IsConnected(i))
                {
                    connectedCount++;

                    SFML.Window.Joystick.Identification id = SFML.Window.Joystick.GetIdentification(i);
                    StaticLogger.Information("PlaystationControllerDetector [ 'Name': {0} , 'ProductId': {1}, 'VendorId': {2} ]",
                        id.Name, id.ProductId, id.VendorId);
                }
            }
            StaticLogger.Information("PlaystationControllerDetector {0} controllers detected, of an allowed {1}", connectedCount, SFML.Window.Joystick.Count);
        }

        public static bool IsPlaystationControllerConnected()
        {
            // refresh SFML's joystick list, else it be reported that there are 0 controllers/joysticks connected
            SFML.Window.Joystick.Update();

            for (uint i = 0; i < SFML.Window.Joystick.Count; i++)
            {
                if (SFML.Window.Joystick.IsConnected(i))
                {
                    SFML.Window.Joystick.Identification id = SFML.Window.Joystick.GetIdentification(i);
                    if (IsPlaystationController(id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsPlaystationController(SFML.Window.Joystick.Identification id)
        {
            return id.VendorId == HidConstants.VENDOR_ID_SONY &&
                (id.ProductId == HidConstants.PRODUCT_ID_PS4_CONTROLLER_A ||
                id.ProductId == HidConstants.PRODUCT_ID_PS4_CONTROLLER_B ||
                id.ProductId == HidConstants.PRODUCT_ID_PS4_CONTROLLER_C ||
                id.ProductId == HidConstants.PRODUCT_ID_PS5_CONTROLLER);
        }

    }
}

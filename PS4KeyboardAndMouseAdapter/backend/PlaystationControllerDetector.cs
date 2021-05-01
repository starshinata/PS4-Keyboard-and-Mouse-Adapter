using Serilog;

namespace PS4KeyboardAndMouseAdapter.backend
{
    class PlaystationControllerDetector
    {
        private static int PS4CONTROLLER_PRODUCT_ID = 2508;
        private static int PS4CONTROLLER_VENDOR_ID = 1356;

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
            return id.ProductId == PS4CONTROLLER_PRODUCT_ID && id.VendorId == PS4CONTROLLER_VENDOR_ID;
        }

    }
}

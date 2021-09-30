using Serilog;

namespace PS4KeyboardAndMouseAdapter.backend
{
    class PlaystationControllerDetector
    {
        // there are more VendorId x ProductId combos at 
        // https://git.sr.ht/~thestr4ng3r/chiaki/tree/master/item/gui/src/controllermanager.cpp

        // "DualShock 4 [CUH-ZCT1x]"
        //                                                aka HEX 05C4
        private static readonly int PRODUCT_ID_PS4_CONTROLLER_A = 1476;

        // "DualShock 4 [CUH-ZCT2x]"
        //                                                aka HEX 09CC
        private static readonly int PRODUCT_ID_PS4_CONTROLLER_B = 2508;

        // "Dualshock4 Wireless Adaptor"
        //                                                aka HEX 0BA0
        private static readonly int PRODUCT_ID_PS4_CONTROLLER_C = 2976;


        //                                              aka HEX 0CE6
        private static readonly int PRODUCT_ID_PS5_CONTROLLER = 3302;

        //                                   aka HEX 054C
        private static readonly int VENDOR_ID_SONY = 1356;

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
            return id.VendorId == VENDOR_ID_SONY &&
                (id.ProductId == PRODUCT_ID_PS4_CONTROLLER_A ||
                id.ProductId == PRODUCT_ID_PS4_CONTROLLER_B ||
                id.ProductId == PRODUCT_ID_PS4_CONTROLLER_C ||
                id.ProductId == PRODUCT_ID_PS5_CONTROLLER);
        }

    }
}

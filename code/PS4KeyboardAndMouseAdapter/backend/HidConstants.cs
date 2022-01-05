namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class HidConstants
    {
        // there are more VendorId x ProductId combos at 
        // https://git.sr.ht/~thestr4ng3r/chiaki/tree/master/item/gui/src/controllermanager.cpp

        // "DualShock 4 [CUH-ZCT1x]"
        //                                                aka HEX 05C4
        public static readonly int PRODUCT_ID_PS4_CONTROLLER_A = 1476;

        // "DualShock 4 [CUH-ZCT2x]"
        //                                                aka HEX 09CC
        public static readonly int PRODUCT_ID_PS4_CONTROLLER_B = 2508;

        // "Dualshock4 Wireless Adaptor"
        //                                                aka HEX 0BA0
        public static readonly int PRODUCT_ID_PS4_CONTROLLER_C = 2976;


        //                                              aka HEX 0CE6
        public static readonly int PRODUCT_ID_PS5_CONTROLLER = 3302;

        //                                   aka HEX 054C
        public static readonly int VENDOR_ID_SONY = 1356;

    }
}

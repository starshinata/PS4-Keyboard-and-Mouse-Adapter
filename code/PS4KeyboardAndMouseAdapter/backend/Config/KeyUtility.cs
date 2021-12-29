using System;
using System.Collections.Generic;
using System.Linq;

namespace PS4KeyboardAndMouseAdapter.Config
{
    class KeyUtility
    {
        private static List<VirtualKey> virtualKeyList;

        public static List<VirtualKey> GetVirtualKeyValues()
        {
            if (virtualKeyList == null)
            {
                Array virtualKeyArray = System.Enum.GetValues(typeof(VirtualKey));
                virtualKeyList = virtualKeyArray.OfType<VirtualKey>().ToList();
                virtualKeyList.Remove(VirtualKey.NULL);
            }
            return virtualKeyList;
        }
    }
}

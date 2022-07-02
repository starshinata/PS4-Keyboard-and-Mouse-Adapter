using Pizza.KeyboardAndMouseAdapter.Backend.Mappings;
using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class ListUtil
    {
        public static string ListToString(List<PhysicalKey> objects)
        {
            string returned = "[";
            if (objects != null)
            {
                returned += string.Join(", ", objects);
            }
            returned += "]";
            return returned;
        }

        public static string ListToString(List<VirtualKey> objects)
        {
            string returned = "[";
            if (objects != null)
            {
                returned += string.Join(", ", objects);
            }
            returned += "]";
            return returned;
        }
    }
}

using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class ListUtil
    {

        public static T First<T>(List<T> objects)
        {
            if (objects != null)
            {
                foreach (T obj in objects)
                {
                    return obj;
                }
            }

            return default(T);
        }

        public static string ListToString<T>(List<T> objects)
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

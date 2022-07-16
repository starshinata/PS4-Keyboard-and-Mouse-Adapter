using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend
{
    class CollectionsUtil
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

        public static T First<T>(HashSet<T> objects)
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

        public static string SetToString<T>(HashSet<T> objects)
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

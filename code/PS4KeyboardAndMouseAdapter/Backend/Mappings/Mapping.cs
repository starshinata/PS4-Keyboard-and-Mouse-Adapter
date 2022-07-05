using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Mappings
{
    public class Mapping
    {
        public int uid;
        public List<PhysicalKey> physicalKeys = new List<PhysicalKey>();
        public List<VirtualKey> virtualKeys = new List<VirtualKey>();

        public string GetCompositeKeyPhysical()
        {
            bool first = true;
            string key = "";

            if (virtualKeys != null)
            {
                foreach (PhysicalKey pk in physicalKeys)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        key += " + ";
                    }
                    key += pk.ToString();
                }
            }
            return key;
        }

        public string GetCompositeKeyVirtual()
        {
            bool first = true;
            string key = "";

            if (virtualKeys != null)
            {
                foreach (VirtualKey vk in virtualKeys)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        key += " + ";
                    }
                    key += vk.ToString();
                }
            }
            return key;
        }
    }
}

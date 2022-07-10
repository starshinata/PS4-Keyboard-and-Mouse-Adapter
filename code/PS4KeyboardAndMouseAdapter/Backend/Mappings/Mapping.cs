using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Mappings
{
    public class Mapping
    {
        public int uid;
        public List<PhysicalKey> PhysicalKeys = new List<PhysicalKey>();
        public List<VirtualKey> VirtualKeys = new List<VirtualKey>();

        public string GetCompositeKeyPhysical()
        {
            bool first = true;
            string key = "";

            if (VirtualKeys != null)
            {
                foreach (PhysicalKey pk in PhysicalKeys)
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

            if (VirtualKeys != null)
            {
                foreach (VirtualKey vk in VirtualKeys)
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

        public bool isSimpleMapping()
        {
            return (PhysicalKeys != null &&
                PhysicalKeys.Count == 1 &&
                VirtualKeys != null &&
                VirtualKeys.Count == 1);
        }
    }
}

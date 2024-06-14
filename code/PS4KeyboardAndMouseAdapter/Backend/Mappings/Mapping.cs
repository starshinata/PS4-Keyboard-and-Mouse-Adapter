using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Mappings
{
    public class Mapping
    {
        public int uid;
        public HashSet<PhysicalKey> PhysicalKeys = new HashSet<PhysicalKey>();
        public HashSet<VirtualKey> VirtualKeys = new HashSet<VirtualKey>();

        public Mapping Clone()
        {
            Mapping newMapping = new Mapping();

            newMapping.uid = uid;

            foreach (PhysicalKey physicalKey in PhysicalKeys)
            {
                newMapping.PhysicalKeys.Add(physicalKey);
            }

            foreach (VirtualKey virtualKey in VirtualKeys)
            {
                newMapping.VirtualKeys.Add(virtualKey);
            }

            return newMapping;
        }

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

        public bool IsSimpleMapping()
        {
            return (PhysicalKeys != null &&
                PhysicalKeys.Count == 1 &&
                VirtualKeys != null &&
                VirtualKeys.Count == 1);
        }
    }
}

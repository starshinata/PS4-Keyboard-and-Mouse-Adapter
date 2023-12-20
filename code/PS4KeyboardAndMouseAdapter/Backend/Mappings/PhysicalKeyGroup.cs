using System.Collections.Generic;

namespace Pizza.KeyboardAndMouseAdapter.Backend.Mappings
{
    public class PhysicalKeyGroup
    {
        public HashSet<PhysicalKey> PhysicalKeys;

        public PhysicalKeyGroup()
        {
            PhysicalKeys = new HashSet<PhysicalKey>();
        }

        public PhysicalKeyGroup(HashSet<PhysicalKey> list)
        {
            PhysicalKeys = list;
        }

        public override string ToString()
        {
            return CollectionsUtil.SetToString(PhysicalKeys);
        }
    }
}

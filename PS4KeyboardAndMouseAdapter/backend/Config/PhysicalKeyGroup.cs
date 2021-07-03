using System.Collections.Generic;

namespace PS4KeyboardAndMouseAdapter.Config
{
    public class PhysicalKeyGroup
    {
        public List<PhysicalKey> PhysicalKeys;

        public PhysicalKeyGroup()
        {
            PhysicalKeys = new List<PhysicalKey>();
        }

        public override string ToString()
        {
            return ListUtil.ListToString(PhysicalKeys);
        }
    }
}

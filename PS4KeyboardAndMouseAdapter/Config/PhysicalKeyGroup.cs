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
            string returned = "";
            if (PhysicalKeys != null)
            {
                returned += "[";
                foreach (PhysicalKey pk in PhysicalKeys)
                {
                    returned += pk + ",";
                }
            }
            returned += "]";

            return returned;
        }
    }
}

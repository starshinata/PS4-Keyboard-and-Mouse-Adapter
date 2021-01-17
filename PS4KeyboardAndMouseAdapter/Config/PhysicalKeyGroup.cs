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
                bool first = true;
                returned += "[";

                foreach (PhysicalKey pk in PhysicalKeys)
                {
                    if (first)
                    {
                        first = false;
                        returned += pk;
                    }
                    else
                    {
                        returned += ", " + pk;
                    }
                }

                returned += "]";
            }

            return returned;
        }
    }
}

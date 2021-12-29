using System.Collections;
using System.Diagnostics;

namespace PS4KeyboardAndMouseAdapter.backend
{
    class ProcessComparerByName : IComparer
    {
        public int Compare(object a, object b)
        {
            return (new CaseInsensitiveComparer()).Compare(((Process)a).ProcessName, ((Process)b).ProcessName);
        }
    }
}

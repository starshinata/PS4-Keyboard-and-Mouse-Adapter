using System.Collections;
using System.Diagnostics;
using System.ServiceProcess;

namespace PS4KeyboardAndMouseAdapter.backend
{
    class ServiceComparerByName : IComparer
    {
        public int Compare(object a, object b)
        {
            return (new CaseInsensitiveComparer()).Compare(((ServiceController)a).ServiceName, ((ServiceController)b).ServiceName);
        }
    }
}

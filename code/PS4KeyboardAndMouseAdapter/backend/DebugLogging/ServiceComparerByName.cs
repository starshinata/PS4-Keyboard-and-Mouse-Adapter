using System.Collections;
using System.ServiceProcess;

namespace Pizza.KeyboardAndMouseAdapter.Backend.DebugLogging
{
    class ServiceComparerByName : IComparer
    {
        public int Compare(object a, object b)
        {
            return (new CaseInsensitiveComparer()).Compare(((ServiceController)a).ServiceName, ((ServiceController)b).ServiceName);
        }
    }
}

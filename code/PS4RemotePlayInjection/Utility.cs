using mscoree;
using Pizza.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PS4RemotePlayInjection
{
    //TODO is this needed any more ?
    public class Utility
    {

        public static IList<AppDomain> GetAppDomains()
        {
            IList<AppDomain> _IList = new List<AppDomain>();
            IntPtr enumHandle = IntPtr.Zero;
            CorRuntimeHostClass host = new mscoree.CorRuntimeHostClass();
            try
            {
                host.EnumDomains(out enumHandle);
                object domain = null;
                while (true)
                {
                    host.NextDomain(enumHandle, out domain);
                    if (domain == null) break;
                    AppDomain appDomain = (AppDomain)domain;
                    _IList.Add(appDomain);
                }
                return _IList;
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException("GetAppDomains error", e);
                return null;
            }
            finally
            {
                host.CloseEnum(enumHandle);
                Marshal.ReleaseComObject(host);
            }
        }
    }
}

using mscoree;
using Pizza.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Pizza.PS4RemotePlayDll.Stuff
{
    public class AppDomainUtility
    {

        public static IList<AppDomain> GetAppDomains()
        {
            IList<AppDomain> _IList = new List<AppDomain>();
            IntPtr enumHandle = IntPtr.Zero;
            CorRuntimeHostClass host = new CorRuntimeHostClass();
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

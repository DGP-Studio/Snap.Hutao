// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;
using Snap.Hutao.Win32.Security;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class FirewallApi
{
    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static extern unsafe uint NetworkIsolationEnumAppContainers(uint Flags, uint* pdwNumPublicAppCs, INET_FIREWALL_APP_CONTAINER** ppPublicAppCs);

    public static unsafe WIN32_ERROR NetworkIsolationEnumAppContainers(NETISO_FLAG Flags, out uint dwNumPublicAppCs, out INET_FIREWALL_APP_CONTAINER* pPublicAppCs)
    {
        fixed (uint* pdwNumPublicAppCs = &dwNumPublicAppCs)
        {
            fixed (INET_FIREWALL_APP_CONTAINER** ppPublicAppCs = &pPublicAppCs)
            {
                uint retVal = NetworkIsolationEnumAppContainers((uint)Flags, pdwNumPublicAppCs, ppPublicAppCs);
                return *(WIN32_ERROR*)&retVal;
            }
        }
    }

    public static unsafe WIN32_ERROR NetworkIsolationEnumAppContainers(NETISO_FLAG Flags, out ReadOnlySpan<INET_FIREWALL_APP_CONTAINER> pPublicAppCs)
    {
        WIN32_ERROR error = NetworkIsolationEnumAppContainers(Flags, out uint dwNumPublicAppCs, out INET_FIREWALL_APP_CONTAINER* pPublicAppCs2);
        pPublicAppCs = new(pPublicAppCs2, (int)dwNumPublicAppCs);
        return error;
    }

    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static extern unsafe uint NetworkIsolationFreeAppContainers(INET_FIREWALL_APP_CONTAINER* pPublicAppCs);

    public static unsafe WIN32_ERROR NetworkIsolationFreeAppContainers(ref readonly INET_FIREWALL_APP_CONTAINER publicAppCs)
    {
        fixed (INET_FIREWALL_APP_CONTAINER* pPublicAppCs = &publicAppCs)
        {
            uint retVal = NetworkIsolationFreeAppContainers(pPublicAppCs);
            return *(WIN32_ERROR*)&retVal;
        }
    }

    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static extern unsafe uint NetworkIsolationGetAppContainerConfig(uint* pdwNumPublicAppCs, SID_AND_ATTRIBUTES** appContainerSids);

    public static unsafe WIN32_ERROR NetworkIsolationGetAppContainerConfig(out uint dwNumPublicAppCs, out SID_AND_ATTRIBUTES* appContainerSids)
    {
        fixed (uint* pdwNumPublicAppCs = &dwNumPublicAppCs)
        {
            fixed (SID_AND_ATTRIBUTES** pAppContainerSids = &appContainerSids)
            {
                uint retVal = NetworkIsolationGetAppContainerConfig(pdwNumPublicAppCs, pAppContainerSids);
                return *(WIN32_ERROR*)&retVal;
            }
        }
    }

    public static unsafe WIN32_ERROR NetworkIsolationGetAppContainerConfig(out ReadOnlySpan<SID_AND_ATTRIBUTES> appContainerSids)
    {
        WIN32_ERROR error = NetworkIsolationGetAppContainerConfig(out uint dwNumPublicAppCs, out SID_AND_ATTRIBUTES* appContainerSids2);
        appContainerSids = new(appContainerSids2, (int)dwNumPublicAppCs);
        return error;
    }

    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static extern unsafe uint NetworkIsolationSetAppContainerConfig(uint dwNumPublicAppCs, SID_AND_ATTRIBUTES* appContainerSids);

    public static unsafe WIN32_ERROR NetworkIsolationSetAppContainerConfig(ReadOnlySpan<SID_AND_ATTRIBUTES> appContainerSids)
    {
        fixed (SID_AND_ATTRIBUTES* pAppContainerSids = appContainerSids)
        {
            uint retVal = NetworkIsolationSetAppContainerConfig((uint)appContainerSids.Length, pAppContainerSids);
            return *(WIN32_ERROR*)&retVal;
        }
    }
}
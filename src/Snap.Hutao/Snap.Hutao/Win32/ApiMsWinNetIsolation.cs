// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;
using Snap.Hutao.Win32.Security;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SH002")]
[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class ApiMsWinNetIsolation
{
    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static unsafe extern uint NetworkIsolationEnumAppContainers(uint Flags, uint* pdwNumPublicAppCs, INET_FIREWALL_APP_CONTAINER** ppPublicAppCs);

    [DebuggerStepThrough]
    public static unsafe uint NetworkIsolationEnumAppContainers(NETISO_FLAG Flags, out uint dwNumPublicAppCs, out INET_FIREWALL_APP_CONTAINER* pPublicAppCs)
    {
        fixed (uint* pdwNumPublicAppCs = &dwNumPublicAppCs)
        {
            fixed (INET_FIREWALL_APP_CONTAINER** ppPublicAppCs = &pPublicAppCs)
            {
                return NetworkIsolationEnumAppContainers((uint)Flags, pdwNumPublicAppCs, ppPublicAppCs);
            }
        }
    }

    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static unsafe extern uint NetworkIsolationFreeAppContainers(INET_FIREWALL_APP_CONTAINER* pPublicAppCs);

    [DebuggerStepThrough]
    public static unsafe uint NetworkIsolationFreeAppContainers(ref readonly INET_FIREWALL_APP_CONTAINER publicAppCs)
    {
        fixed (INET_FIREWALL_APP_CONTAINER* pPublicAppCs = &publicAppCs)
        {
            return NetworkIsolationFreeAppContainers(pPublicAppCs);
        }
    }

    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static unsafe extern uint NetworkIsolationGetAppContainerConfig(uint* pdwNumPublicAppCs, SID_AND_ATTRIBUTES** appContainerSids);

    [DebuggerStepThrough]
    public static unsafe uint NetworkIsolationGetAppContainerConfig(out uint dwNumPublicAppCs, out SID_AND_ATTRIBUTES* appContainerSids)
    {
        fixed (uint* pdwNumPublicAppCs = &dwNumPublicAppCs)
        {
            fixed (SID_AND_ATTRIBUTES** pAppContainerSids = &appContainerSids)
            {
                return NetworkIsolationGetAppContainerConfig(pdwNumPublicAppCs, pAppContainerSids);
            }
        }
    }

    [DllImport("api-ms-win-net-isolation-l1-1-0.dll", ExactSpelling = true)]
    [SupportedOSPlatform("windows8.0")]
    public static unsafe extern uint NetworkIsolationSetAppContainerConfig(uint dwNumPublicAppCs, SID_AND_ATTRIBUTES* appContainerSids);

    [DebuggerStepThrough]
    public static unsafe uint NetworkIsolationSetAppContainerConfig(ReadOnlySpan<SID_AND_ATTRIBUTES> appContainerSids)
    {
        fixed (SID_AND_ATTRIBUTES* pAppContainerSids = appContainerSids)
        {
            return NetworkIsolationSetAppContainerConfig((uint)appContainerSids.Length, pAppContainerSids);
        }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Registry;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class AdvApi32
{
    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern WIN32_ERROR RegCloseKey(HKEY hKey);

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern WIN32_ERROR RegNotifyChangeKeyValue(HKEY hKey, BOOL bWatchSubtree, REG_NOTIFY_FILTER dwNotifyFilter, [Optional] HANDLE hEvent, BOOL fAsynchronous);

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe WIN32_ERROR RegOpenKeyExW(HKEY hKey, [Optional] PCWSTR lpSubKey, [Optional] uint ulOptions, REG_SAM_FLAGS samDesired, HKEY* phkResult);

    public static unsafe WIN32_ERROR RegOpenKeyExW(HKEY hKey, ReadOnlySpan<char> subKey, uint ulOptions, REG_SAM_FLAGS samDesired, out HKEY hkResult)
    {
        fixed (char* lpSubKey = subKey)
        {
            fixed (HKEY* phkResult = &hkResult)
            {
                return RegOpenKeyExW(hKey, lpSubKey, ulOptions, samDesired, phkResult);
            }
        }
    }
}
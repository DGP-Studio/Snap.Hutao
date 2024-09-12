﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Registry;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SA1313")]
[SuppressMessage("", "SYSLIB1054")]
internal static class AdvApi32
{
    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern unsafe BOOL ConvertSidToStringSidW(PSID Sid, PWSTR* StringSid);

    public static unsafe BOOL ConvertSidToStringSidW(PSID Sid, out PWSTR StringSid)
    {
        fixed (PWSTR* pStringSid = &StringSid)
        {
            return ConvertSidToStringSidW(Sid, pStringSid);
        }
    }

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern unsafe BOOL ConvertStringSidToSidW(PCWSTR StringSid, PSID* Sid);

    public static unsafe BOOL ConvertStringSidToSidW(ReadOnlySpan<char> StringSid, out PSID Sid)
    {
        fixed (char* pStringSid = StringSid)
        {
            fixed (PSID* pSid = &Sid)
            {
                return ConvertStringSidToSidW(pStringSid, pSid);
            }
        }
    }

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern BOOL EqualSid(PSID pSid1, PSID pSid2);

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern WIN32_ERROR RegCloseKey(HKEY hKey);

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern WIN32_ERROR RegNotifyChangeKeyValue(HKEY hKey, BOOL bWatchSubtree, REG_NOTIFY_FILTER dwNotifyFilter, [AllowNull] HANDLE hEvent, BOOL fAsynchronous);

    [DllImport("ADVAPI32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.0")]
    public static extern unsafe WIN32_ERROR RegOpenKeyExW(HKEY hKey, [AllowNull] PCWSTR lpSubKey, [AllowNull] uint ulOptions, REG_SAM_FLAGS samDesired, HKEY* phkResult);

    [DebuggerStepThrough]
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
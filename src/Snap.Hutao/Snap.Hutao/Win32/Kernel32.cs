// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.Security;
using Snap.Hutao.Win32.Storage.FileSystem;
using Snap.Hutao.Win32.System.Console;
using Snap.Hutao.Win32.System.IO;
using Snap.Hutao.Win32.System.LibraryLoader;
using Snap.Hutao.Win32.System.Memory;
using Snap.Hutao.Win32.System.ProcessStatus;
using Snap.Hutao.Win32.System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SYSLIB1054")]
internal static class Kernel32
{
    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern WIN32_ERROR GetLastError();

    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern HMODULE GetModuleHandleW([Optional] PCWSTR lpModuleName);

    [DebuggerStepThrough]
    public static unsafe HMODULE GetModuleHandleW(ReadOnlySpan<char> moduleName)
    {
        fixed (char* lpModuleName = moduleName)
        {
            return GetModuleHandleW(lpModuleName);
        }
    }
}
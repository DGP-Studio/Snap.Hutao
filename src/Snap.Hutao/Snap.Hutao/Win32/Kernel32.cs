// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Snap.Hutao.Win32;

[SuppressMessage("", "SYSLIB1054")]
internal static class Kernel32
{
    [DllImport("KERNEL32.dll", CallingConvention = CallingConvention.Winapi, ExactSpelling = true)]
    [SupportedOSPlatform("windows5.1.2600")]
    public static extern WIN32_ERROR GetLastError();
}
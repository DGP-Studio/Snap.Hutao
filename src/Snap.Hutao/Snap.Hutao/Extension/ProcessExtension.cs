// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Extension;

internal static class ProcessExtension
{
    public static bool IsRunning(this Process process)
    {
        try
        {
            return !process.HasExited;
        }
        catch (COMException)
        {
            // 句柄无效。 (0x80070006 (E_HANDLE))
            return false;
        }
        catch (InvalidOperationException)
        {
            // No process is associated with this object.
            return false;
        }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Extension;

internal static class ProcessExtension
{
    public static bool IsRunning(this Process process)
    {
        try
        {
            return !process.HasExited;
        }
        catch (InvalidOperationException)
        {
            // No process is associated with this object.
            return false;
        }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Diagnostics;

internal static class ProcessExtension
{
    public static bool IsRunning(this IProcess process)
    {
        try
        {
            return !process.HasExited;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return false;
        }
    }

    public static void SafeWaitForExit(this IProcess process)
    {
        try
        {
            process.WaitForExit();
        }
        catch (Exception e)
        {
            SentrySdk.CaptureException(e);
        }
    }
}
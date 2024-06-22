// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

internal sealed class DefaultGameFpsUnlocker : GameFpsUnlocker
{
    public DefaultGameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess, in UnlockOptions options, IProgress<GameFpsUnlockerContext> progress)
        : base(serviceProvider, gameProcess, options, progress)
    {
    }

    protected override async ValueTask PostUnlockOverrideAsync(GameFpsUnlockerContext context, LaunchOptions launchOptions, ILogger logger, CancellationToken token = default)
    {
        using (PeriodicTimer timer = new(context.Options.AdjustFpsDelay))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                if (!context.GameProcess.HasExited && context.FpsAddress != 0U)
                {
                    UnsafeWriteProcessMemory(context.AllAccess, context.FpsAddress, launchOptions.TargetFps);
                    WIN32_ERROR error = GetLastError();
                    if (error is not WIN32_ERROR.NO_ERROR)
                    {
                        logger.LogError("Failed to WriteProcessMemory at FpsAddress, error code 0x{Code:X8}", error);
                        context.Description = SH.FormatServiceGameUnlockerWriteProcessMemoryFpsAddressFailed(error);
                    }

                    context.Report();
                }
                else
                {
                    context.IsUnlockerValid = false;
                    context.FpsAddress = 0;
                    context.Report();
                    return;
                }
            }
        }
    }

    [SuppressMessage("", "SH002")]
    private static unsafe bool UnsafeWriteProcessMemory(HANDLE hProcess, nuint baseAddress, int value)
    {
        return WriteProcessMemory(hProcess, (void*)baseAddress, ref value, out _);
    }
}
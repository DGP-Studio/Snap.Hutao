// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Win32.Foundation;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.Service.Game.Unlocker;

/// <summary>
/// 游戏帧率解锁器
/// Credit to https://github.com/34736384/genshin-fps-unlock
/// </summary>
[HighQuality]
internal sealed class GameFpsUnlocker : IGameFpsUnlocker
{
    private readonly LaunchOptions launchOptions;
    private readonly GameFpsUnlockerContext context = new();

    public GameFpsUnlocker(IServiceProvider serviceProvider, Process gameProcess, in UnlockTimingOptions options, IProgress<GameFpsUnlockerContext> progress)
    {
        launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();

        context.GameProcess = gameProcess;
        context.TimingOptions = options;
        context.Progress = progress;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> UnlockAsync(CancellationToken token = default)
    {
        HutaoException.ThrowIfNot(context.IsUnlockerValid, HutaoExceptionKind.GameFpsUnlockingFailed, "This Unlocker is invalid");
        (FindModuleResult result, RequiredGameModule gameModule) = await GameProcessModule.FindModuleAsync(context).ConfigureAwait(false);
        HutaoException.ThrowIfNot(result != FindModuleResult.TimeLimitExeeded, HutaoExceptionKind.GameFpsUnlockingFailed, SH.ServiceGameUnlockerFindModuleTimeLimitExeeded);
        HutaoException.ThrowIfNot(result != FindModuleResult.NoModuleFound, HutaoExceptionKind.GameFpsUnlockingFailed, SH.ServiceGameUnlockerFindModuleNoModuleFound);

        GameFpsAddress.UnsafeFindFpsAddress(context, gameModule);
        context.Report();
        return context.FpsAddress != 0U;
    }

    public async ValueTask PostUnlockAsync(CancellationToken token = default)
    {
        using (PeriodicTimer timer = new(context.TimingOptions.AdjustFpsDelay))
        {
            while (await timer.WaitForNextTickAsync(token).ConfigureAwait(false))
            {
                if (!context.GameProcess.HasExited && context.FpsAddress != 0U)
                {
                    UnsafeWriteProcessMemory(context.GameProcess, context.FpsAddress, launchOptions.TargetFps);
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

    private static unsafe bool UnsafeWriteProcessMemory(Process process, nuint baseAddress, int value)
    {
        return WriteProcessMemory((HANDLE)process.Handle, (void*)baseAddress, ref value, out _);
    }
}
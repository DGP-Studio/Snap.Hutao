﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Unlocker;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionUnlockFpsHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled)
        {
            context.Logger.LogInformation("Unlocking FPS");
            context.Progress.Report(new(LaunchPhase.UnlockingFps, SH.ServiceGameLaunchPhaseUnlockingFps));

            if (!context.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
            {
                return;
            }

            if (!gameFileSystem.TryGetGameVersion(out string? gameVerison))
            {
                return;
            }

            GameFpsUnlocker unlocker = new(context.ServiceProvider, context.Process, gameVerison);

            try
            {
                if (await unlocker.UnlockAsync(context.CancellationToken).ConfigureAwait(false))
                {
                    await TaskExtension.WhenAllOrAnyException(unlocker.PostUnlockAsync(context.CancellationToken).AsTask(), next().AsTask()).ConfigureAwait(false);
                }
                else
                {
                    HutaoException.Throw("下载解锁帧率配置文件失败");
                }
            }
            catch (Exception ex)
            {
                context.Result.Kind = LaunchExecutionResultKind.GameFpsUnlockingFailed;
                context.Result.ErrorMessage = ex.Message;

                // The Unlocker can't unlock the process
                context.Process.Kill();
            }
        }
        else
        {
            context.Logger.LogInformation("Unlock FPS skipped");
            await next().ConfigureAwait(false);
        }
    }
}
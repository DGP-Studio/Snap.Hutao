// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Island;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameIslandHandler : ILaunchExecutionDelegateHandler
{
    private readonly bool resume;

    public LaunchExecutionGameIslandHandler(bool resume)
    {
        this.resume = resume;
    }

    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled)
        {
            context.Progress.Report(new(LaunchPhase.UnlockingFps, SH.ServiceGameLaunchPhaseUnlockingFps));

            try
            {
                GameIslandInterop interop = new(context, resume);
                if (await interop.PrepareAsync().ConfigureAwait(false))
                {
                    await TaskExtension.WhenAllOrAnyException(interop.WaitForExitAsync().AsTask(), next().AsTask()).ConfigureAwait(false);
                }
                else
                {
                    HutaoException.Throw("Failed to download island feature configuration.");
                }
            }
            catch (Exception ex)
            {
                context.Result.Kind = LaunchExecutionResultKind.GameIslandOperationFailed;
                context.Result.ErrorMessage = ex.Message;
                context.Process.Kill();// The Unlocker can't unlock the process
            }
        }
        else
        {
            context.Logger.LogInformation("Unlock FPS skipped");
            await next().ConfigureAwait(false);
        }
    }
}
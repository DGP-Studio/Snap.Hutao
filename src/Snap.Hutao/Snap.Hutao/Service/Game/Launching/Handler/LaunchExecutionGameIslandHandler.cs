// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Game.Island;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameIslandHandler : ILaunchExecutionDelegateHandler
{
    private readonly bool resume;
    private GameIslandInterop? interop;

    public LaunchExecutionGameIslandHandler(bool resume)
    {
        this.resume = resume;
    }

    public async ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled.Value)
        {
            interop = new(context, resume);
            try
            {
                context.Progress.Report(new(LaunchPhase.IslandStaging, SH.ServiceGameLaunchPhaseUnlockingFps));
                if (!await interop.PrepareAsync().ConfigureAwait(false))
                {
                    if (!string.IsNullOrEmpty(context.Result.ErrorMessage))
                    {
                        context.Result.Kind = LaunchExecutionResultKind.GameIslandOperationFailed;
                    }
                    else
                    {
                        HutaoException.Throw("Failed to download island feature configuration.");
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                context.Result.Kind = LaunchExecutionResultKind.GameIslandOperationFailed;
                context.Result.ErrorMessage = ex.Message;
                return false;
            }

            context.Progress.Report(default);
        }

        return await next().ConfigureAwait(false);
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        if (HutaoRuntime.IsProcessElevated && context.Options.IsIslandEnabled.Value)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(interop);
                await TaskExtension.WhenAllOrAnyException(interop.WaitForExitAsync().AsTask(), next().AsTask()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                context.Result.Kind = LaunchExecutionResultKind.GameIslandOperationFailed;
                context.Result.ErrorMessage = ex.Message;
                context.Process.Kill();
            }
        }
        else
        {
            await next().ConfigureAwait(false);
        }
    }
}
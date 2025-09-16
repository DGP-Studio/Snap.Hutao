// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Service.Game.Island;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Notification;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameIslandHandler : AbstractLaunchExecutionHandler
{
    private readonly bool resume;
    private GameIslandInterop? interop;

    public LaunchExecutionGameIslandHandler(bool resume)
    {
        this.resume = resume;
    }

    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (!HutaoRuntime.IsProcessElevated || !context.LaunchOptions.IsIslandEnabled.Value)
        {
            return;
        }

        interop = new(resume);

        if (!resume || GameLifeCycle.IsGameRunning())
        {
            context.Progress.Report(new(SH.ServiceGameLaunchPhaseUnlockingFps));
        }

        await interop.BeforeAsync(context).ConfigureAwait(false);
        context.Progress.Report(default);
    }

    public override ValueTask ExecuteAsync(LaunchExecutionContext context)
    {
        if (!HutaoRuntime.IsProcessElevated || !context.LaunchOptions.IsIslandEnabled.Value)
        {
            return ValueTask.CompletedTask;
        }

        ExecuteCoreAsync(context).SafeForget();
        return ValueTask.CompletedTask;
    }

    private async ValueTask ExecuteCoreAsync(LaunchExecutionContext context)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(interop);
            await interop.WaitForExitAsync(context).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            context.Messenger.Send(InfoBarMessage.Error(ex));
            context.Process.Kill();
        }
    }
}
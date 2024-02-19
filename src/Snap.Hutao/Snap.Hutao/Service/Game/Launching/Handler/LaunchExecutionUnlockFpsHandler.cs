// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Unlocker;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionUnlockFpsHandler : ILaunchExecutionDelegateHandler
{
    public async ValueTask OnExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        RuntimeOptions runtimeOptions = context.ServiceProvider.GetRequiredService<RuntimeOptions>();
        if (runtimeOptions.IsElevated && context.Options.IsAdvancedLaunchOptionsEnabled && context.Options.UnlockFps)
        {
            context.Logger.LogInformation("Unlocking FPS");
            context.Progress.Report(new(LaunchPhase.UnlockingFps, SH.ServiceGameLaunchPhaseUnlockingFps));

            IProgressFactory progressFactory = context.ServiceProvider.GetRequiredService<IProgressFactory>();
            IProgress<UnlockerStatus> progress = progressFactory.CreateForMainThread<UnlockerStatus>(status => context.Progress.Report(LaunchStatus.FromUnlockStatus(status)));
            GameFpsUnlocker unlocker = context.ServiceProvider.CreateInstance<GameFpsUnlocker>(context.Process);

            try
            {
                await unlocker.UnlockAsync(new(100, 20000, 3000), progress, context.CancellationToken).ConfigureAwait(false);
            }
            catch (InvalidOperationException ex)
            {
                context.Logger.LogCritical(ex, "Unlocking FPS failed");

                context.Result.Kind = LaunchExecutionResultKind.GameFpsUnlockingFailed;
                context.Result.ErrorMessage = ex.Message;

                // The Unlocker can't unlock the process
                context.Process.Kill();
                return;
            }
        }

        await next().ConfigureAwait(false);
    }
}
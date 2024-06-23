// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Factory.Progress;
using Snap.Hutao.Service.Game.Unlocker;
using Snap.Hutao.Service.Game.Unlocker.Island;

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
            IProgress<GameFpsUnlockerContext> progress = progressFactory.CreateForMainThread<GameFpsUnlockerContext>(c => context.Progress.Report(LaunchStatus.FromUnlockerContext(c)));
            if (!context.TryGetGameFileSystem(out GameFileSystem? gameFileSystem))
            {
                return;
            }

            IslandGameFpsUnlocker unlocker = new(context.ServiceProvider, context.Process, new(gameFileSystem, 100, 20000, 2000), progress);

            try
            {
                if (await unlocker.UnlockAsync(context.CancellationToken).ConfigureAwait(false))
                {
                    unlocker.PostUnlockAsync(context.CancellationToken).SafeForget(context.Logger);
                }
                else
                {
                    context.Logger.LogError("Unlocking FPS failed");
                }
            }
            catch (Exception ex)
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
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Discord;

namespace Snap.Hutao.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionSetDiscordActivityHandler : ILaunchExecutionDelegateHandler
{
    public ValueTask<bool> BeforeExecutionAsync(LaunchExecutionContext context, BeforeExecutionDelegate next)
    {
        return next();
    }

    public async ValueTask ExecutionAsync(LaunchExecutionContext context, LaunchExecutionDelegate next)
    {
        bool previousSetDiscordActivityWhenPlaying = context.Options.SetDiscordActivityWhenPlaying.Value;

        try
        {
            if (previousSetDiscordActivityWhenPlaying)
            {
                context.Logger.LogInformation("Set discord activity as playing");
                await context.ServiceProvider
                    .GetRequiredService<IDiscordService>()
                    .SetPlayingActivityAsync(context.TargetScheme.IsOversea)
                    .ConfigureAwait(false);
            }

            await next().ConfigureAwait(false);
        }
        finally
        {
            if (previousSetDiscordActivityWhenPlaying)
            {
                context.Logger.LogInformation("Recover discord activity");
                await context.ServiceProvider
                    .GetRequiredService<IDiscordService>()
                    .SetNormalActivityAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}
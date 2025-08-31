// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Factory.Process;

namespace Snap.Hutao.Service.Discord;

[Service(ServiceLifetime.Singleton, typeof(IDiscordService))]
internal sealed partial class DiscordService : IDiscordService, IDisposable
{
    public async ValueTask SetPlayingActivityAsync(bool isOversea)
    {
        if (IsSupported())
        {
            _ = isOversea
                ? await DiscordController.SetPlayingGenshinImpactAsync().ConfigureAwait(false)
                : await DiscordController.SetPlayingYuanShenAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask SetNormalActivityAsync()
    {
        if (IsSupported())
        {
            _ = await DiscordController.SetDefaultActivityAsync(HutaoRuntime.LaunchTime).ConfigureAwait(false);
        }
    }

    public void Dispose()
    {
        DiscordController.Stop();
    }

    private static bool IsSupported()
    {
        // Actually requires a discord client to be running on Windows platform.
        // If not, discord core creation code will throw.
        return ProcessFactory.IsRunning("Discord", "Discord Updater");
    }
}
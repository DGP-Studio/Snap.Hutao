// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Service.Discord;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDiscordService))]
internal sealed partial class DiscordService : IDiscordService, IDisposable
{
    private readonly RuntimeOptions runtimeOptions;

    public async ValueTask SetPlayingActivityAsync(bool isOversea)
    {
        _ = isOversea
            ? await DiscordController.SetPlayingGenshinImpactAsync().ConfigureAwait(false)
            : await DiscordController.SetPlayingYuanShenAsync().ConfigureAwait(false);
    }

    public async ValueTask SetNormalActivityAsync()
    {
        _ = await DiscordController.SetDefaultActivityAsync(runtimeOptions.AppLaunchTime).ConfigureAwait(false);
    }

    public void Dispose()
    {
        DiscordController.Stop();
    }
}
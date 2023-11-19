// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Discord;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IDiscordService))]
internal sealed partial class DiscordService : IDiscordService, IDisposable
{
    public void Dispose()
    {
        DiscordController.Stop();
    }
}
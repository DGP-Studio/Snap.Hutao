// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Discord;

internal interface IDiscordService
{
    ValueTask SetNormalActivityAsync();

    ValueTask SetPlayingActivityAsync(bool isOversea);
}
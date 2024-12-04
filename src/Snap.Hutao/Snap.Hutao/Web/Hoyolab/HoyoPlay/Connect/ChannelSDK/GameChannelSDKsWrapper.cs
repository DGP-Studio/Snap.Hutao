// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;

internal sealed class GameChannelSDKsWrapper
{
    [JsonPropertyName("game_channel_sdks")]
    public List<GameChannelSDK> GameChannelSDKs { get; set; } = default!;
}

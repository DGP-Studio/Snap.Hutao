// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;

internal sealed class ChannelSDKs
{
    [JsonPropertyName("game_channel_sdks")]
    public List<ChannelSDK> GameChannelSDKs { get; set; } = default!;
}

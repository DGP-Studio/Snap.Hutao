// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game.Scheme;

internal sealed class LaunchSchemeBilibili : LaunchScheme
{
    private const string HoyoPlayLauncherBilibiliId = "umfgRO5gh5";
    private const string HoyoPlayGameBilibiliId = "T2S0Gz4Dr2";

    public LaunchSchemeBilibili(SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = HoyoPlayLauncherBilibiliId;
        GameId = HoyoPlayGameBilibiliId;
        Channel = ChannelType.Bili;
        SubChannel = subChannel;
        IsOversea = false;
        IsNotCompatOnly = isNotCompatOnly;
    }
}
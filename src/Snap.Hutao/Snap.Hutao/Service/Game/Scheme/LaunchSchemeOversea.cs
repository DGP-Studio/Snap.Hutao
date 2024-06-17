// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game.Scheme;

internal sealed class LaunchSchemeOversea : LaunchScheme
{
    private const int SdkStaticLauncherOverseaId = 10;
    private const string SdkStaticLauncherOverseaKey = "gcStgarh";
    private const string HoyoPlayLauncherOverseaId = "VYTpXlbWo8";
    private const string HoyoPlayGameOverseaId = "gopR6Cufr3";

    public LaunchSchemeOversea(ChannelType channel, SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = SdkStaticLauncherOverseaId;
        Key = SdkStaticLauncherOverseaKey;
        HoyoPlayLauncherId = HoyoPlayLauncherOverseaId;
        HoyoPlayGameId = HoyoPlayGameOverseaId;
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = true;
        IsNotCompatOnly = isNotCompatOnly;
    }
}
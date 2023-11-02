// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game;

internal sealed class LaunchSchemeOversea : LaunchScheme
{
    private const int SdkStaticLauncherOverseaId = 10;
    private const string SdkStaticLauncherOverseaKey = "gcStgarh";

    public LaunchSchemeOversea(ChannelType channel, SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = SdkStaticLauncherOverseaId;
        Key = SdkStaticLauncherOverseaKey;
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = true;
        IsNotCompatOnly = isNotCompatOnly;
    }
}
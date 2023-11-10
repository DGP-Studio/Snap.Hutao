// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game.Scheme;

internal sealed class LaunchSchemeChinese : LaunchScheme
{
    private const int SdkStaticLauncherChineseId = 18;
    private const string SdkStaticLauncherChineseKey = "eYd89JmJ";

    public LaunchSchemeChinese(ChannelType channel, SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = SdkStaticLauncherChineseId;
        Key = SdkStaticLauncherChineseKey;
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = false;
        IsNotCompatOnly = isNotCompatOnly;
    }
}
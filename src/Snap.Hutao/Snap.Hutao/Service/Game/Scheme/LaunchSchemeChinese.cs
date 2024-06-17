// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game.Scheme;

internal sealed class LaunchSchemeChinese : LaunchScheme
{
    private const int SdkStaticLauncherChineseId = 18;
    private const string SdkStaticLauncherChineseKey = "eYd89JmJ";
    private const string HoyoPlayLauncherChineseId = "jGHBHlcOq1";
    private const string HoyoPlayGameChineseId = "1Z8W5NHUQb";

    public LaunchSchemeChinese(ChannelType channel, SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = SdkStaticLauncherChineseId;
        Key = SdkStaticLauncherChineseKey;
        HoyoPlayLauncherId = HoyoPlayLauncherChineseId;
        HoyoPlayGameId = HoyoPlayGameChineseId;
        Channel = channel;
        SubChannel = subChannel;
        IsOversea = false;
        IsNotCompatOnly = isNotCompatOnly;
    }
}
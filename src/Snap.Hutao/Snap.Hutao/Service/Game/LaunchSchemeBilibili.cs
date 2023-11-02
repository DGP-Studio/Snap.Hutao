// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Service.Game;

internal sealed class LaunchSchemeBilibili : LaunchScheme
{
    private const int SdkStaticLauncherBilibiliId = 17;
    private const string SdkStaticLauncherBilibiliKey = "KAtdSsoQ";

    public LaunchSchemeBilibili(SubChannelType subChannel, bool isNotCompatOnly = true)
    {
        LauncherId = SdkStaticLauncherBilibiliId;
        Key = SdkStaticLauncherBilibiliKey;
        Channel = ChannelType.Bili;
        SubChannel = subChannel;
        IsOversea = false;
        IsNotCompatOnly = isNotCompatOnly;
    }
}
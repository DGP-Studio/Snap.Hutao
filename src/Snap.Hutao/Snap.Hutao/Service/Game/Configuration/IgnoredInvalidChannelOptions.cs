// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Frozen;

namespace Snap.Hutao.Service.Game.Configuration;

internal static class IgnoredInvalidChannelOptions
{
    private static readonly FrozenSet<ChannelOptions> InvalidOptions =
    [
        new ChannelOptions(ChannelType.Bili, SubChannelType.Default, isOversea: true),
        new ChannelOptions(ChannelType.Bili, SubChannelType.Official, isOversea: true),
        new ChannelOptions(ChannelType.Official, SubChannelType.Google, isOversea: false),
    ];

    public static bool Contains(in ChannelOptions options)
    {
        return InvalidOptions.Contains(options);
    }
}
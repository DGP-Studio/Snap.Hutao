// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Game.Configuration;

internal static class IgnoredInvalidChannelOptions
{
    private static readonly ImmutableHashSet<ChannelOptions> InvalidOptions = new HashSet<ChannelOptions>()
    {
        new(ChannelType.Bili, SubChannelType.Official, true),
    }.ToImmutableHashSet();

    public static bool Contains(in ChannelOptions options)
    {
        return InvalidOptions.Contains(options);
    }
}

// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Model.Metadata;

internal sealed class GachaEvent
{
    public required string Name { get; init; }

    public required string Version { get; init; }

    public required uint Order { get; init; }

    public required Uri Banner { get; init; }

    public Uri? Banner2 { get; init; }

    public required DateTimeOffset From { get; init; }

    public required DateTimeOffset To { get; init; }

    public required GachaType Type { get; init; }

    public required HashSet<uint> UpOrangeList { get; init; }

    public required HashSet<uint> UpPurpleList { get; init; }
}
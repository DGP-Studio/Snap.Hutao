// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Tower;

internal sealed class TowerSchedule : IDefaultIdentity<TowerScheduleId>
{
    public required TowerScheduleId Id { get; init; }

    public required ImmutableArray<TowerFloorId> FloorIds { get; init; }

    public required DateTimeOffset Open { get; init; }

    public required DateTimeOffset Close { get; init; }

    public required string BuffName { get; init; }

    public required ImmutableArray<string> Descriptions { get; init; }

    public required string Icon { get; init; }
}
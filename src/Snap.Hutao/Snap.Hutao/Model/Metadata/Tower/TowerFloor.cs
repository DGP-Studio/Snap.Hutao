// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Tower;

internal sealed class TowerFloor : IDefaultIdentity<TowerFloorId>
{
    public required TowerFloorId Id { get; init; }

    public required uint Index { get; init; }

    public required TowerLevelGroupId LevelGroupId { get; init; }

    public required string Background { get; init; }

    public required ImmutableArray<string> Descriptions { get; init; }

    public ImmutableArray<string> FirstDescriptions { get; init; }

    public ImmutableArray<string> SecondDescriptions { get; init; }
}
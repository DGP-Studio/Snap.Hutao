// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Tower;

internal sealed class TowerLevel
{
    public required TowerLevelId Id { get; init; }

    public required TowerLevelGroupId GroupId { get; init; }

    public required uint Index { get; init; }

    public required uint MonsterLevel { get; init; }

    public ImmutableArray<MonsterDescribeId> FirstMonsters { get; init; }

    public ImmutableArray<TowerWave> FirstWaves { get; init; }

    public NameDescription? FirstGadget { get; init; }

    public ImmutableArray<MonsterDescribeId> SecondMonsters { get; init; }

    public ImmutableArray<TowerWave> SecondWaves { get; init; }

    public NameDescription? SecondGadget { get; init; }
}
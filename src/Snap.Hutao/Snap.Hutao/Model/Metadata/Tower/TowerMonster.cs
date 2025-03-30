// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Tower;

internal sealed class TowerMonster
{
    public required MonsterDescribeId Id { get; init; }

    public required uint Count { get; init; }

    public bool AttackMonolith { get; init; }

    public ImmutableArray<NameDescription> Affixes { get; init; }
}
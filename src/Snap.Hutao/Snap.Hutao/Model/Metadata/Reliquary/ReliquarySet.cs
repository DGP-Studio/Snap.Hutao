// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Reliquary;

internal sealed class ReliquarySet
{
    public required ReliquarySetId SetId { get; init; }

    public required EquipAffixId EquipAffixId { get; init; }

    public required ImmutableHashSet<ExtendedEquipAffixId> EquipAffixIds { get; init; }

    public required string Name { get; init; }

    public required string Icon { get; init; }

    public required ImmutableArray<int> NeedNumber { get; init; }

    public required ImmutableArray<string> Descriptions { get; init; }
}
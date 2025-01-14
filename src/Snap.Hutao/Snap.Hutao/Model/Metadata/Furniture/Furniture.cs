// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Furniture;

internal sealed class Furniture
{
    public required ImmutableArray<FurnitureTypeId> Types { get; init; }

    public required FurnitureDeploySurfaceType SurfaceType { get; init; }

    public required bool IsSpecial { get; init; }

    public required SpecialFurnitureType SpecialType { get; init; }

    public required uint Comfort { get; init; }

    public required uint Cost { get; init; }

    public required uint DiscountCost { get; init; }

    public required bool CanFloat { get; init; }

    public required bool IsUnique { get; init; }

    public string? ItemIcon { get; init; }

    public string? EffectIcon { get; init; }

    public required QualityType RankLevel { get; init; }

    public required ImmutableArray<FurnitureId> GroupUnits { get; init; }

    public required GroupRecordType GroupRecordType { get; init; }

    public required ImmutableArray<string> SourceTexts { get; init; }

    public required FurnitureId Id { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public string? Icon { get; init; }

    public required uint Rank { get; init; }
}
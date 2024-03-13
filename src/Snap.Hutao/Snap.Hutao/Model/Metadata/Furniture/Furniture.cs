// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Furniture;

internal sealed class Furniture
{
    public List<FurnitureTypeId> Types { get; set; } = default!;

    public FurnitureDeploySurfaceType SurfaceType { get; set; }

    public bool IsSpecial { get; set; }

    public SpecialFurnitureType SpecialType { get; set; }

    public uint Comfort { get; set; }

    public uint Cost { get; set; }

    public uint DiscountCost { get; set; }

    public bool CanFloat { get; set; }

    public bool IsUnique { get; set; }

    public string? ItemIcon { get; set; }

    public string? EffectIcon { get; set; }

    public QualityType RankLevel { get; set; }

    public List<FurnitureId> GruopUnits { get; set; } = default!;

    public GroupRecordType GroupRecordType { get; set; }

    public List<string> SourceTexts { get; set; } = default!;

    public FurnitureId Id { get; set; }

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string? Icon { get; set; }

    public uint Rank { get; set; }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Item;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Model.Metadata.Monster;

internal sealed partial class Monster : IAdvancedCollectionViewItem
{
    internal const uint MaxLevel = 110U;

    public required MonsterId Id { get; init; }

    public required MonsterDescribeId DescribeId { get; init; }

    public required MonsterRelationshipId RelationshipId { get; init; }

    public string? MonsterName { get; init; }

    public string? Name { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    public required string Icon { get; init; }

    public required MonsterType Type { get; init; }

    public required Arkhe Arkhe { get; init; }

    public List<string>? Affixes { get; init; }

    public List<MaterialId>? Drops { get; init; }

    public MonsterBaseValue? BaseValue { get; init; }

    public TypeValueCollection<FightProperty, GrowCurveType>? GrowCurves { get; init; }

    [JsonIgnore]
    public List<DisplayItem>? DropsView { get; set; }
}
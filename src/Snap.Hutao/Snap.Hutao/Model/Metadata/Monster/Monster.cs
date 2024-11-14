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

    public MonsterId Id { get; set; }

    public MonsterDescribeId DescribeId { get; set; }

    public MonsterRelationshipId RelationshipId { get; set; }

    public string MonsterName { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string Icon { get; set; } = default!;

    public MonsterType Type { get; set; }

    public Arkhe Arkhe { get; set; }

    public List<string>? Affixes { get; set; } = default!;

    public List<MaterialId>? Drops { get; set; } = default!;

    public MonsterBaseValue BaseValue { get; set; } = default!;

    public TypeValueCollection<FightProperty, GrowCurveType> GrowCurves { get; set; } = default!;

    [JsonIgnore]
    public List<DisplayItem>? DropsView { get; set; }
}
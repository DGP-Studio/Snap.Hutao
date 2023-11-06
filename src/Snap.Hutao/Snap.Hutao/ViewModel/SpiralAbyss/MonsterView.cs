// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class MonsterView : INameIcon, IMappingFrom<MonsterView, TowerMonster, Model.Metadata.Monster.Monster>
{
    private MonsterView(MonsterRelationshipId id)
    {
        Name = $"Unknown {id}";
        Icon = Web.HutaoEndpoints.UIIconNone;
        Count = 1;
    }

    private MonsterView(TowerMonster towerMonster, Model.Metadata.Monster.Monster metaMonster)
    {
        Name = metaMonster.Name;
        Icon = MonsterIconConverter.IconNameToUri(metaMonster.Icon);
        Affixes = towerMonster.Affixes;
        Count = (int)towerMonster.Count;
        AttackMonolith = towerMonster.AttackMonolith;
    }

    public static MonsterView Default(MonsterRelationshipId id)
    {
        return new(id);
    }

    public string Name { get; }

    public Uri Icon { get; }

    public List<NameDescription>? Affixes { get; }

    public int Count { get; }

    public bool IsCountOne { get => Count == 1; }

    public bool AttackMonolith { get; }

    public static MonsterView From(TowerMonster tower, Model.Metadata.Monster.Monster meta)
    {
        return new MonsterView(tower, meta);
    }
}
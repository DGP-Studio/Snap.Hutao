// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class BattleWave
{
    private BattleWave(TowerWave towerWave, SpiralAbyssMetadataContext context)
    {
        Description = towerWave.Type.GetLocalizedDescriptionOrDefault() ?? SH.ModelMetadataTowerWaveTypeDefault;
        Monsters = towerWave.Monsters.SelectArray(m => CreateMonsterViewOrDefault(m, context));
    }

    public string Description { get; }

    public ImmutableArray<MonsterView> Monsters { get; }

    public static BattleWave From(TowerWave tower, SpiralAbyssMetadataContext context)
    {
        return new(tower, context);
    }

    private static MonsterView CreateMonsterViewOrDefault(TowerMonster towerMonster, SpiralAbyssMetadataContext context)
    {
        MonsterRelationshipId normalizedId = MonsterRelationship.Normalize(towerMonster.Id);
        return context.IdMonsterMap.TryGetValue(normalizedId, out Monster? metadataMonster)
            ? MonsterView.From(towerMonster, metadataMonster)
            : MonsterView.Default(normalizedId);
    }
}
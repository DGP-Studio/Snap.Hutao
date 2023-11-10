// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class BattleWave : IMappingFrom<BattleWave, TowerWave, SpiralAbyssMetadataContext>
{
    private BattleWave(TowerWave towerWave, SpiralAbyssMetadataContext context)
    {
        Description = towerWave.Type.GetLocalizedDescriptionOrDefault() ?? SH.ModelMetadataTowerWaveTypeDefault;
        Monsters = towerWave.Monsters.SelectList(m => CreateMonsterViewOrDefault(m, context));
    }

    public string Description { get; }

    public List<MonsterView> Monsters { get; }

    public static BattleWave From(TowerWave tower, SpiralAbyssMetadataContext context)
    {
        return new(tower, context);
    }

    private static MonsterView CreateMonsterViewOrDefault(TowerMonster towerMonster, SpiralAbyssMetadataContext context)
    {
        MonsterRelationshipId normalizedId = MonsterRelationship.Normalize(towerMonster.Id);
        return context.IdMonsterMap.TryGetValue(normalizedId, out Model.Metadata.Monster.Monster? metadataMonster)
            ? MonsterView.From(towerMonster, metadataMonster)
            : MonsterView.Default(normalizedId);
    }
}
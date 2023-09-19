// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Metadata;
using Snap.Hutao.Model.Metadata.Tower;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class BattleWave : IMappingFrom<BattleWave, TowerWave, SpiralAbyssMetadataContext>
{
    private BattleWave(TowerWave towerWave, SpiralAbyssMetadataContext context)
    {
        Description = towerWave.Type.GetLocalizedDescriptionOrDefault() ?? SH.ModelMetadataTowerWaveTypeDefault;
        Monsters = towerWave.Monsters.SelectList(m => MonsterView.From(m, context.IdMonsterMap[MonsterRelationship.Normalize(m.Id)]));
    }

    public string Description { get; }

    public List<MonsterView> Monsters { get; }

    public static BattleWave From(TowerWave tower, SpiralAbyssMetadataContext context)
    {
        return new(tower, context);
    }
}
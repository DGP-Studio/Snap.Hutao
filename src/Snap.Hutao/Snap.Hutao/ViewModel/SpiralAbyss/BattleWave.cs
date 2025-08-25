// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Monster;
using Snap.Hutao.Model.Metadata.Tower;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;
using System.Globalization;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

internal sealed class BattleWave
{
    private BattleWave(TowerWave towerWave, SpiralAbyssMetadataContext context)
    {
        Description = towerWave.Type.GetLocalizedDescriptionOrDefault(SH.ResourceManager, CultureInfo.CurrentCulture) ?? SH.ModelMetadataTowerWaveTypeDefault;
        Monsters = towerWave.Monsters.SelectAsArray(static (m, context) => CreateMonsterViewOrDefault(m, context), context);
    }

    public string Description { get; }

    public ImmutableArray<MonsterView> Monsters { get; }

    public static BattleWave Create(TowerWave tower, SpiralAbyssMetadataContext context)
    {
        return new(tower, context);
    }

    private static MonsterView CreateMonsterViewOrDefault(TowerMonster towerMonster, SpiralAbyssMetadataContext context)
    {
        MonsterDescribeId normalizedId = MonsterDescribe.Normalize(towerMonster.Id);
        return context.IdMonsterMap.TryGetValue(normalizedId, out Monster? metadataMonster)
            ? MonsterView.Create(towerMonster, metadataMonster)
            : MonsterView.Default(normalizedId);
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Metadata.Tower;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 间视图
/// </summary>
[HighQuality]
internal sealed class LevelView : IMappingFrom<LevelView, TowerLevel, SpiralAbyssMetadataContext>
{
    private LevelView(TowerLevel towerLevel, SpiralAbyssMetadataContext context)
    {
        Index = SH.ModelBindingHutaoComplexRankLevel.Format(towerLevel.Index);
        IndexValue = towerLevel.Index;
        Battles = new()
        {
            BattleView.From(towerLevel, 1, context),
            BattleView.From(towerLevel, 2, context),
        };
    }

    /// <summary>
    /// 间号
    /// </summary>
    public string Index { get; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; private set; }

    /// <summary>
    /// 上下半
    /// </summary>
    public List<BattleView> Battles { get; private set; }

    internal uint IndexValue { get; set; }

    public static LevelView From(TowerLevel towerLevel, SpiralAbyssMetadataContext context)
    {
        return new(towerLevel, context);
    }

    public void WithSpiralAbyssLevel(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Level level, SpiralAbyssMetadataContext context)
    {
        Star = level.Star;

        foreach (BattleView battleView in Battles)
        {
            if (level.Battles.SingleOrDefault(b => b.Index == battleView.IndexValue) is { } battle)
            {
                battleView.WithSpiralAbyssBattle(battle, context);
            }
        }
    }
}
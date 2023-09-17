// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model.Metadata.Tower;

namespace Snap.Hutao.ViewModel.SpiralAbyss;

/// <summary>
/// 层视图
/// </summary>
[HighQuality]
internal sealed class FloorView : IMappingFrom<FloorView, TowerFloor, SpiralAbyssMetadataContext>
{
    public FloorView(TowerFloor floor, SpiralAbyssMetadataContext context)
    {
        Index = SH.ModelBindingHutaoComplexRankFloor.Format(floor.Index);
        IndexValue = floor.Index;
        Disorders = floor.Descriptions;

        Levels = context.IdLevelGroupMap[floor.LevelGroupId].SortBy(l => l.Index).SelectList(l => LevelView.From(l, context));
    }

    public bool Engaged { get; private set; }

    /// <summary>
    /// 层号
    /// </summary>
    public string Index { get; }

    /// <summary>
    /// 时间
    /// </summary>
    public string? SettleTime { get; private set; }

    /// <summary>
    /// 星数
    /// </summary>
    public int Star { get; private set; }

    public List<string> Disorders { get; }

    /// <summary>
    /// 间信息
    /// </summary>
    public List<LevelView> Levels { get; }

    internal uint IndexValue { get; }

    public static FloorView From(TowerFloor floor, SpiralAbyssMetadataContext context)
    {
        return new(floor, context);
    }

    public void WithSpiralAbyssFloor(Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.Floor floor, SpiralAbyssMetadataContext context)
    {
        SettleTime = $"{DateTimeOffset.FromUnixTimeSeconds(floor.SettleTime).ToLocalTime():yyyy.MM.dd HH:mm:ss}";
        Star = floor.Star;
        Engaged = true;

        foreach (LevelView levelView in Levels)
        {
            if (floor.Levels.SingleOrDefault(l => l.Index == levelView.IndexValue) is { } level)
            {
                levelView.WithSpiralAbyssLevel(level, context);
            }
        }
    }
}
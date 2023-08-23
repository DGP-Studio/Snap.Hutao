// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 统计数据
/// </summary>
[HighQuality]
internal sealed class Overview
{
    /// <summary>
    /// 规划Id
    /// </summary>
    public int ScheduleId { get; set; }

    /// <summary>
    /// 总记录数
    /// </summary>
    public int RecordTotal { get; set; }

    /// <summary>
    /// 总深渊计数
    /// </summary>
    public int SpiralAbyssTotal { get; set; }

    /// <summary>
    /// 通关玩家总数
    /// </summary>
    public int SpiralAbyssPassed { get; set; }

    /// <summary>
    /// 通关玩家比例
    /// </summary>
    [JsonIgnore]
    public string SpiralAbyssPassedPercent { get => $"{(double)SpiralAbyssPassed / SpiralAbyssTotal:P2}"; }

    /// <summary>
    /// 总星数
    /// </summary>
    public int SpiralAbyssStarTotal { get; set; }

    /// <summary>
    /// 平均星数
    /// </summary>
    [JsonIgnore]
    public string SpiralAbyssStarAverage { get => $"{(double)SpiralAbyssStarTotal / SpiralAbyssTotal:F2}"; }

    /// <summary>
    /// 满星数
    /// </summary>
    public int SpiralAbyssFullStar { get; set; }

    /// <summary>
    /// 满星比例
    /// </summary>
    [JsonIgnore]
    public string SpiralAbyssFullStarPercent { get => $"{(double)SpiralAbyssFullStar / SpiralAbyssTotal:P2}"; }

    /// <summary>
    /// 总战斗次数
    /// </summary>
    public long SpiralAbyssBattleTotal { get; set; }

    /// <summary>
    /// 平均战斗次数
    /// </summary>
    [JsonIgnore]
    public string SpiralAbyssBattleAverage { get => $"{(double)SpiralAbyssBattleTotal / SpiralAbyssTotal:F2}"; }

    /// <summary>
    /// 统计时间
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// 数据刷新的时间
    /// </summary>
    [JsonIgnore]
    public string RefreshTime { get => $"{DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).ToLocalTime():MM.dd HH:mm}"; }

    /// <summary>
    /// 总时间
    /// </summary>
    public double TimeTotal { get; set; }

    /// <summary>
    /// 平均时间
    /// </summary>
    public double TimeAverage { get; set; }
}
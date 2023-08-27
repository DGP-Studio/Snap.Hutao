// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.SpiralAbyss.Post;

/// <summary>
/// 深渊数据
/// </summary>
[HighQuality]
internal sealed class SimpleSpiralAbyss
{
    /// <summary>
    /// 构造一个新的深渊信息
    /// </summary>
    /// <param name="spiralAbyss">深渊信息</param>
    public SimpleSpiralAbyss(Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss)
    {
        ScheduleId = spiralAbyss.ScheduleId;
        TotalBattleTimes = spiralAbyss.TotalBattleTimes;
        TotalWinTimes = spiralAbyss.TotalWinTimes;
        Damage = SimpleRank.FromRank(spiralAbyss.DamageRank.SingleOrDefault());
        TakeDamage = SimpleRank.FromRank(spiralAbyss.TakeDamageRank.SingleOrDefault());
        Floors = spiralAbyss.Floors.Select(f => new SimpleFloor(f));
    }

    /// <summary>
    /// 计划Id
    /// </summary>
    public int ScheduleId { get; set; }

    /// <summary>
    /// 总战斗次数
    /// </summary>
    public int TotalBattleTimes { get; set; }

    /// <summary>
    /// 总战斗胜利次数
    /// </summary>
    public int TotalWinTimes { get; set; }

    /// <summary>
    /// 造成伤害
    /// </summary>
    public SimpleRank? Damage { get; set; } = default!;

    /// <summary>
    /// 受到伤害
    /// </summary>
    public SimpleRank? TakeDamage { get; set; } = default!;

    /// <summary>
    /// 层
    /// </summary>
    public IEnumerable<SimpleFloor> Floors { get; set; } = default!;
}
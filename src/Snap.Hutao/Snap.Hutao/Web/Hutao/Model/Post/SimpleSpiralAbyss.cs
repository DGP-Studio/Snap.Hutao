// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

namespace Snap.Hutao.Web.Hutao.Model.Post;

/// <summary>
/// 深渊数据
/// </summary>
public class SimpleSpiralAbyss
{
    /// <summary>
    /// 构造一个新的深渊信息
    /// </summary>
    /// <param name="spiralAbyss">深渊信息</param>
    public SimpleSpiralAbyss(SpiralAbyss spiralAbyss)
    {
        ScheduleId = spiralAbyss.ScheduleId;
        Damage = new(spiralAbyss.DamageRank.Single());
        TakeDamage = new(spiralAbyss.TakeDamageRank.Single());
        Floors = spiralAbyss.Floors.Select(f => new SimpleFloor(f));
    }

    /// <summary>
    /// 计划Id
    /// </summary>
    public int ScheduleId { get; set; }

    /// <summary>
    /// 造成伤害
    /// </summary>
    public SimpleRank Damage { get; set; } = default!;

    /// <summary>
    /// 受到伤害
    /// </summary>
    public SimpleRank TakeDamage { get; set; } = default!;

    /// <summary>
    /// 层
    /// </summary>
    public IEnumerable<SimpleFloor> Floors { get; set; } = default!;
}
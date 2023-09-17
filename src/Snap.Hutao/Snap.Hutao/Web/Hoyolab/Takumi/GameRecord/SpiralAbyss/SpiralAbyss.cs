// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

/// <summary>
/// 深境螺旋信息
/// </summary>
[HighQuality]
internal sealed class SpiralAbyss
{
    /// <summary>
    /// 计划Id
    /// </summary>
    [JsonPropertyName("schedule_id")]
    public uint ScheduleId { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    /// <summary>
    /// 战斗次数
    /// </summary>
    [JsonPropertyName("total_battle_times")]
    public int TotalBattleTimes { get; set; }

    /// <summary>
    /// 胜利次数
    /// </summary>
    [JsonPropertyName("total_win_times")]
    public int TotalWinTimes { get; set; }

    /// <summary>
    /// 最深抵达
    /// </summary>
    [JsonPropertyName("max_floor")]
    public string MaxFloor { get; set; } = default!;

    /// <summary>
    /// 出战次数
    /// </summary>
    [JsonPropertyName("reveal_rank")]
    public List<Rank> RevealRank { get; set; } = default!;

    /// <summary>
    /// 击破次数
    /// </summary>
    [JsonPropertyName("defeat_rank")]
    public List<Rank> DefeatRank { get; set; } = default!;

    /// <summary>
    /// 最强一击
    /// </summary>
    [JsonPropertyName("damage_rank")]
    public List<Rank> DamageRank { get; set; } = default!;

    /// <summary>
    /// 承受伤害
    /// </summary>
    [JsonPropertyName("take_damage_rank")]
    public List<Rank> TakeDamageRank { get; set; } = default!;

    /// <summary>
    /// 元素战技
    /// </summary>
    [JsonPropertyName("normal_skill_rank")]
    public List<Rank> NormalSkillRank { get; set; } = default!;

    /// <summary>
    /// 元素爆发
    /// </summary>
    [JsonPropertyName("energy_skill_rank")]
    public List<Rank> EnergySkillRank { get; set; } = default!;

    /// <summary>
    /// 层信息
    /// </summary>
    [JsonPropertyName("floors")]
    public List<Floor> Floors { get; set; } = default!;

    /// <summary>
    /// 共获得渊星
    /// </summary>
    [JsonPropertyName("total_star")]
    public int TotalStar { get; set; }

    /// <summary>
    /// 是否解锁
    /// </summary>
    [JsonPropertyName("is_unlock")]
    public bool IsUnlock { get; set; }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyss
{
    [JsonPropertyName("schedule_id")]
    public uint ScheduleId { get; set; }

    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }

    [JsonPropertyName("end_time")]
    public long EndTime { get; set; }

    [JsonPropertyName("total_battle_times")]
    public int TotalBattleTimes { get; set; }

    [JsonPropertyName("total_win_times")]
    public int TotalWinTimes { get; set; }

    [JsonPropertyName("max_floor")]
    public string MaxFloor { get; set; } = default!;

    [JsonPropertyName("reveal_rank")]
    public List<SpiralAbyssRank> RevealRank { get; set; } = default!;

    [JsonPropertyName("defeat_rank")]
    public List<SpiralAbyssRank> DefeatRank { get; set; } = default!;

    [JsonPropertyName("damage_rank")]
    public List<SpiralAbyssRank> DamageRank { get; set; } = default!;

    [JsonPropertyName("take_damage_rank")]
    public List<SpiralAbyssRank> TakeDamageRank { get; set; } = default!;

    [JsonPropertyName("normal_skill_rank")]
    public List<SpiralAbyssRank> NormalSkillRank { get; set; } = default!;

    [JsonPropertyName("energy_skill_rank")]
    public List<SpiralAbyssRank> EnergySkillRank { get; set; } = default!;

    [JsonPropertyName("floors")]
    public List<SpiralAbyssFloor> Floors { get; set; } = default!;

    [JsonPropertyName("total_star")]
    public int TotalStar { get; set; }

    [JsonPropertyName("is_unlock")]
    public bool IsUnlock { get; set; }
}
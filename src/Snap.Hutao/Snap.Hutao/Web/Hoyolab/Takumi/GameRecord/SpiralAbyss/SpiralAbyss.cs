// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

internal sealed class SpiralAbyss
{
    [JsonPropertyName("schedule_id")]
    public required uint ScheduleId { get; init; }

    [JsonPropertyName("start_time")]
    public required long StartTime { get; init; }

    [JsonPropertyName("end_time")]
    public required long EndTime { get; init; }

    [JsonPropertyName("total_battle_times")]
    public required int TotalBattleTimes { get; init; }

    [JsonPropertyName("total_win_times")]
    public required int TotalWinTimes { get; init; }

    [JsonPropertyName("max_floor")]
    public required string MaxFloor { get; init; } = default!;

    [JsonPropertyName("reveal_rank")]
    public required ImmutableArray<SpiralAbyssRank> RevealRank { get; init; }

    [JsonPropertyName("defeat_rank")]
    public required ImmutableArray<SpiralAbyssRank> DefeatRank { get; init; }

    [JsonPropertyName("damage_rank")]
    public required ImmutableArray<SpiralAbyssRank> DamageRank { get; init; }

    [JsonPropertyName("take_damage_rank")]
    public required ImmutableArray<SpiralAbyssRank> TakeDamageRank { get; init; }

    [JsonPropertyName("normal_skill_rank")]
    public required ImmutableArray<SpiralAbyssRank> NormalSkillRank { get; init; }

    [JsonPropertyName("energy_skill_rank")]
    public required ImmutableArray<SpiralAbyssRank> EnergySkillRank { get; init; }

    [JsonPropertyName("floors")]
    public required ImmutableArray<SpiralAbyssFloor> Floors { get; init; }

    [JsonPropertyName("total_star")]
    public required int TotalStar { get; init; }

    [JsonPropertyName("is_unlock")]
    public required bool IsUnlock { get; init; }
}
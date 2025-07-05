// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatFightStatistics
{
    [JsonPropertyName("max_defeat_avatar")]
    public RoleCombatAvatarStatistics? MaxDefeatAvatar { get; init; }

    [JsonPropertyName("max_damage_avatar")]
    public RoleCombatAvatarStatistics? MaxDamageAvatar { get; init; }

    [JsonPropertyName("max_take_damage_avatar")]
    public RoleCombatAvatarStatistics? MaxTakeDamageAvatar { get; init; }

    [JsonPropertyName("total_coin_consumed")]
    public RoleCombatAvatarStatistics? TotalCoinConsumed { get; init; }

    [JsonPropertyName("shortest_avatar_list")]
    public required ImmutableArray<RoleCombatAvatarStatistics> ShortestAvatarList { get; init; }

    [JsonPropertyName("total_use_time")]
    public required int TotalUseTime { get; init; }

    [JsonPropertyName("is_show_battle_stats")]
    public required bool IsShowBattleStats { get; init; }
}
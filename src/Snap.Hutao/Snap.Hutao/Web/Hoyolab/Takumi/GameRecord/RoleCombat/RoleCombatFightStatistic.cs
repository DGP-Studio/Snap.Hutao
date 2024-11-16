// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatFightStatistic
{
    [JsonPropertyName("max_defeat_avatar")]
    public RoleCombatAvatarDamage MaxDefeatAvatar { get; set; } = default!;

    [JsonPropertyName("max_damage_avatar")]
    public RoleCombatAvatarDamage MaxDamageAvatar { get; set; } = default!;

    [JsonPropertyName("max_take_damage_avatar")]
    public RoleCombatAvatarDamage MaxTakeDamageAvatar { get; set; } = default!;

    [JsonPropertyName("total_coin_consumed")]
    public RoleCombatAvatarDamage TotalCoinConsumed { get; set; } = default!;

    [JsonPropertyName("shortest_avatar_list")]
    public List<RoleCombatAvatarDamage> ShortestAvatarList { get; set; } = default!;

    [JsonPropertyName("total_use_time")]
    public int TotalUseTime { get; set; }

    [JsonPropertyName("is_show_battle_stats")]
    public bool IsShowBattleStats { get; set; }
}
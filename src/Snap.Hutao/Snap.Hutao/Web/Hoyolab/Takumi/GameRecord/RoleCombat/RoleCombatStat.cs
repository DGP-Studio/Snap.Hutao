// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatStat
{
    [JsonPropertyName("difficulty_id")]
    public uint DifficultyId { get; set; }

    [JsonPropertyName("max_round_id")]
    public uint MaxRoundId { get; set; }

    [JsonPropertyName("heraldry")]
    public int Heraldry { get; set; }

    [JsonPropertyName("get_medal_round_list")]
    public List<int> GetMedalRoundList { get; set; } = default!;

    [JsonPropertyName("medal_num")]
    public int MedalNumber { get; set; }

    [JsonPropertyName("coin_num")]
    public int CoinNumber { get; set; }

    [JsonPropertyName("avatar_bonus_num")]
    public int AvatarBonusNumber { get; set; }

    [JsonPropertyName("rent_cnt")]
    public int RentCount { get; set; }
}
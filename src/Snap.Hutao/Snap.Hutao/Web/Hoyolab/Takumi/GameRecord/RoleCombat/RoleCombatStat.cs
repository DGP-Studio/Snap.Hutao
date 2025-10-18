// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatStat
{
    [JsonPropertyName("difficulty_id")]
    public required RoleCombatDifficultyLevel DifficultyId { get; init; }

    [JsonPropertyName("max_round_id")]
    public required uint MaxRoundId { get; init; }

    [JsonPropertyName("heraldry")]
    public required RoleCombatDifficultyLevel Heraldry { get; init; }

    [JsonPropertyName("get_medal_round_list")]
    public required ImmutableArray<int> GetMedalRoundList { get; init; }

    [JsonPropertyName("medal_num")]
    public required int MedalNumber { get; init; }

    [JsonPropertyName("coin_num")]
    public required int CoinNumber { get; init; }

    [JsonPropertyName("avatar_bonus_num")]
    public required int AvatarBonusNumber { get; init; }

    [JsonPropertyName("rent_cnt")]
    public required int RentCount { get; init; }

    [JsonPropertyName("tarot_finished_cnt")]
    public int TarotFinishedCount { get; set; }
}
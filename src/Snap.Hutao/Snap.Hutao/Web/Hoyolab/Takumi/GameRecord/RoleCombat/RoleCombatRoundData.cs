// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatRoundData
{
    [JsonPropertyName("avatars")]
    public required ImmutableArray<RoleCombatAvatar> Avatars { get; init; }

    [JsonPropertyName("choice_cards")]
    public required ImmutableArray<RoleCombatBuff> ChoiceCards { get; init; }

    [JsonPropertyName("buffs")]
    public required ImmutableArray<RoleCombatBuff> Buffs { get; init; }

    [JsonPropertyName("is_get_medal")]
    public required bool IsGetMedal { get; init; }

    [JsonPropertyName("round_id")]
    public required uint RoundId { get; init; }

    [JsonPropertyName("finish_time")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required long FinishTime { get; init; }

    [JsonPropertyName("finish_date_time")]
    public required DateTime FinishDateTime { get; init; }

    [JsonPropertyName("enemies")]
    public required ImmutableArray<RoleCombatEnemy> Enemies { get; init; }

    [JsonPropertyName("splendour_buff")]
    public required RoleCombatSplendourBuffWrapper SplendourBuff { get; init; }

    [JsonPropertyName("is_tarot")]
    public bool IsTarot { get; init; }

    [JsonPropertyName("tarot_serial_no")]
    public int TarotSerialNumber { get; init; }
}
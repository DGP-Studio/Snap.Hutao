// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatRoundData
{
    [JsonPropertyName("avatars")]
    public List<RoleCombatAvatar> Avatars { get; set; } = default!;

    [JsonPropertyName("choice_cards")]
    public List<RoleCombatBuff> ChoiceCards { get; set; } = default!;

    [JsonPropertyName("buffs")]
    public List<RoleCombatBuff> Buffs { get; set; } = default!;

    [JsonPropertyName("enemies")]
    public List<RoleCombatEnemy> Enemies { get; set; } = default!;

    [JsonPropertyName("is_get_medal")]
    public bool IsGetMedal { get; set; }

    [JsonPropertyName("round_id")]
    public uint RoundId { get; set; }

    [JsonPropertyName("finish_time")]
    public long FinishTime { get; set; }

    [JsonPropertyName("finish_date_time")]
    public DateTime FinishDateTime { get; set; } = default!;

    [JsonPropertyName("splendour_buff")]
    public RoleCombatSplendourBuffWrapper SplendourBuff { get; set; } = default!;
}
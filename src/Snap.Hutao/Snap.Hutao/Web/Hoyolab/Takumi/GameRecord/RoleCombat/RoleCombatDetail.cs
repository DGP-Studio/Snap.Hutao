// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatDetail
{
    [JsonPropertyName("rounds_data")]
    public required ImmutableArray<RoleCombatRoundData> RoundsData { get; init; }

    [JsonPropertyName("detail_stat")]
    public required RoleCombatStat? DetailStat { get; init; }

    [JsonPropertyName("lineup_link")]
    public Uri? LineupLink { get; init; }

    [JsonPropertyName("backup_avatars")]
    public required ImmutableArray<RoleCombatAvatar> BackupAvatars { get; init; }

    // sic
    [JsonPropertyName("fight_statisic")]
    public required RoleCombatFightStatistics FightStatistics { get; init; }
}
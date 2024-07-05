// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatDetail
{
    [JsonPropertyName("rounds_data")]
    public List<RoleCombatRoundData> RoundsData { get; set; } = default!;

    [JsonPropertyName("detail_stat")]
    public RoleCombatStat? DetailStat { get; set; }

    [JsonPropertyName("backup_avatars")]
    public List<RoleCombatAvatar> BackupAvatars { get; set; } = default!;
}
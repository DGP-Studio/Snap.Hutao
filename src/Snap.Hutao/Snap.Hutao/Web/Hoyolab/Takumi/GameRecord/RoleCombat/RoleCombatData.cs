// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class RoleCombatData
{
    [JsonPropertyName("detail")]
    public RoleCombatDetail Detail { get; set; } = default!;

    [JsonPropertyName("stat")]
    public RoleCombatStat Stat { get; set; } = default!;

    [JsonPropertyName("schedule")]
    public RoleCombatSchedule Schedule { get; set; } = default!;

    [JsonPropertyName("has_data")]
    public bool HasData { get; set; }

    [JsonPropertyName("has_detail_data")]
    [MemberNotNullWhen(true, nameof(Detail))]
    public bool HasDetailData { get; set; }
}
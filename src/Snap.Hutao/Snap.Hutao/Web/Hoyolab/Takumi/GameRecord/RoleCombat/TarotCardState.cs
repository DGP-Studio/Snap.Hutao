// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

internal sealed class TarotCardState : ListWrapper<TarotCard>
{
    [JsonPropertyName("total_num")]
    public required int TotalNumber { get; init; }

    [JsonPropertyName("curr_num")]
    public required int CurrentNumber { get; init; }
}
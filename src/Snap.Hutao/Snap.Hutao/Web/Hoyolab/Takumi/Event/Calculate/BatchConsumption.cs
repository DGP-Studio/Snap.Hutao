// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class BatchConsumption
{
    [JsonPropertyName("items")]
    public ImmutableArray<Consumption> Items { get; set; }

    [JsonPropertyName("overall_consume")]
    public ImmutableArray<Item> OverallConsume { get; set; }

    [JsonPropertyName("has_user_info")]
    public bool HasUserInfo { get; set; }
}
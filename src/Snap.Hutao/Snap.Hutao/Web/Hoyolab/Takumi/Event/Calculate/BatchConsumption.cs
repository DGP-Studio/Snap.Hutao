// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class BatchConsumption
{
    [JsonPropertyName("items")]
    public ImmutableArray<Consumption> Items { get; set; }

    [JsonPropertyName("available_material")]
    public ImmutableArray<Item> AvailableMaterial { get; set; }

    [JsonPropertyName("overall_consume")]
    public ImmutableArray<Item> OverallConsume { get; set; }

    [JsonPropertyName("has_user_info")]
    public bool HasUserInfo { get; set; }

    public static BatchConsumption CreateForBatch(ImmutableArray<Consumption> items)
    {
        return new()
        {
            Items = items,
        };
    }

    public static BatchConsumption CreateForWiki(ImmutableArray<Item> items)
    {
        return new()
        {
            OverallConsume = items,
        };
    }
}
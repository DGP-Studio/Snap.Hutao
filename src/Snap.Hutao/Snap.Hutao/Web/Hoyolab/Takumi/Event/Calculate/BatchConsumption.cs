// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class BatchConsumption
{
    [JsonPropertyName("items")]
    public List<Consumption> Items { get; set; } = default!;

    [JsonPropertyName("available_material")]
    public List<Item>? AvailableMaterial { get; set; }

    [JsonPropertyName("overall_consume")]
    public List<Item> OverallConsume { get; set; } = default!;

    [JsonPropertyName("has_user_info")]
    public bool HasUserInfo { get; set; }
}
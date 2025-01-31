// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Strategy;

internal sealed class Strategy
{
    [JsonPropertyName("mys_strategy_id")]
    public int? MysStrategyId { get; set; }

    [JsonPropertyName("hoyolab_strategy_id")]
    public int? HoyolabStrategyId { get; set; }
}
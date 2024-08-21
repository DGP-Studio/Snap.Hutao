// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

internal sealed class RecommendRelicProperty
{
    [JsonPropertyName("recommend_properties")]
    public RecommendProperties RecommendProperties { get; set; }

    // Ignored field custom_properties
    // Ignored field has_set_recommend_prop
}
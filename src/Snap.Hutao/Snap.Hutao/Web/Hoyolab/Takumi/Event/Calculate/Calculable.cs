// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal class Calculable : PromotionDelta
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("icon")]
    public Uri Icon { get; set; } = default!;

    [JsonPropertyName("max_level")]
    public int MaxLevel { get; set; }

    public virtual PromotionDelta ToPromotionDelta()
    {
        return new()
        {
            LevelCurrent = LevelCurrent,
            LevelTarget = LevelTarget,
            Id = Id,
        };
    }
}
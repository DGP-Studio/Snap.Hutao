// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class BatchSkillCosumption
{
    [JsonPropertyName("consume_list")]
    public List<Item> ConsumeList { get; set; } = default!;

    [JsonPropertyName("skill_info")]
    public PromotionDelta SkillInfo { get; set; } = default!;
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 圣遗物
/// </summary>
internal sealed class Reliquary : Calculable
{
    /// <summary>
    /// 圣遗物分组Id
    /// </summary>
    [JsonPropertyName("reliquary_cat_id")]
    public int ReliquaryCatId { get; set; }

    /// <summary>
    /// 圣遗物品质
    /// </summary>
    [JsonPropertyName("reliquary_level")]
    public int ReliquaryLevel { get; set; }
}
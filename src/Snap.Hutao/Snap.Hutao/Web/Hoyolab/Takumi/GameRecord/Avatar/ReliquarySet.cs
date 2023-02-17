// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 圣遗物套装信息
/// </summary>
[HighQuality]
internal sealed class ReliquarySet
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 套装名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 套装效果
    /// </summary>
    [JsonPropertyName("affixes")]
    public List<ReliquaryAffix> Affixes { get; set; } = default!;
}

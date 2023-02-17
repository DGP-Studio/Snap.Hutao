// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;

/// <summary>
/// 角色的基础信息
/// </summary>
[HighQuality]
internal class Avatar
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public AvatarId Id { get; set; }

    /// <summary>
    /// 图片Url
    /// </summary>
    [JsonPropertyName("image")]
    public string Image { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 元素英文名称
    /// </summary>
    [JsonPropertyName("element")]
    public string Element { get; set; } = default!;

    /// <summary>
    /// 好感度
    /// </summary>
    [JsonPropertyName("fetter")]
    public int Fetter { get; set; }

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 稀有度
    /// </summary>
    [JsonPropertyName("rarity")]
    public ItemQuality Rarity { get; set; }

    /// <summary>
    /// 激活的命座数
    /// </summary>
    [JsonPropertyName("actived_constellation_num")]
    public int ActivedConstellationNum { get; set; }
}
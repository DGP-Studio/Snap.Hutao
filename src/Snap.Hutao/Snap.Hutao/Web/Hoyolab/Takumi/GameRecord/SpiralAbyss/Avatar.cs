// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;

/// <summary>
/// 仅包含头像的角色信息
/// </summary>
public class Avatar
{
    /// <summary>
    /// Id
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 稀有度
    /// </summary>
    [JsonPropertyName("rarity")]
    public int Rarity { get; set; }
}
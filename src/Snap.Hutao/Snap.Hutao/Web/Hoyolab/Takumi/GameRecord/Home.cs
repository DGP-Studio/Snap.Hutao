// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 家园信息
/// </summary>
public class Home
{
    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 历史访客数
    /// </summary>
    [JsonPropertyName("visit_num")]
    public int VisitNum { get; set; }

    /// <summary>
    /// 最高洞天仙力
    /// </summary>
    [JsonPropertyName("comfort_num")]
    public int ComfortNum { get; set; }

    /// <summary>
    /// 获得摆设数
    /// </summary>
    [JsonPropertyName("item_num")]
    public int ItemNum { get; set; }

    /// <summary>
    /// 洞天名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 洞天等级名称
    /// </summary>
    [JsonPropertyName("comfort_level_name")]
    public string ComfortLevelName { get; set; } = default!;

    /// <summary>
    /// 洞天等级图标
    /// </summary>
    [JsonPropertyName("comfort_level_icon")]
    public string ComfortLevelIcon { get; set; } = default!;
}
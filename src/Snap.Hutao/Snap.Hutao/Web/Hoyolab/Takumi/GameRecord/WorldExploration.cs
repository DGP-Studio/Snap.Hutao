// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 世界探索
/// </summary>
public class WorldExploration
{
    /// <summary>
    /// 等级
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 探索度
    /// Maxmium is 1000
    /// </summary>
    [JsonPropertyName("exploration_percentage")]
    public int ExplorationPercentage { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 类型
    /// Offering
    /// Reputation
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    /// <summary>
    /// 供奉进度
    /// </summary>
    [JsonPropertyName("offerings")]
    public List<Offering> Offerings { get; set; } = default!;

    /// <summary>
    /// ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>
    /// 父ID 当无链接的父对象时为0
    /// </summary>
    [JsonPropertyName("parent_id")]
    public int ParentId { get; set; }

    /// <summary>
    /// 地图链接
    /// </summary>
    [JsonPropertyName("map_url")]
    public string MapUrl { get; set; } = default!;

    /// <summary>
    /// 攻略链接 无攻略时为 <see cref="string.Empty"/>
    /// </summary>
    [JsonPropertyName("strategy_url")]
    public string StrategyUrl { get; set; } = default!;

    /// <summary>
    /// 背景图片
    /// </summary>
    [JsonPropertyName("background_image")]
    public string BackgroundImage { get; set; } = default!;

    /// <summary>
    /// 反色图标
    /// </summary>
    [JsonPropertyName("inner_icon")]
    public string InnerIcon { get; set; } = default!;

    /// <summary>
    /// 背景图片
    /// </summary>
    [JsonPropertyName("cover")]
    public string Cover { get; set; } = default!;

    /// <summary>
    /// 百分比*100进度
    /// </summary>
    public double ExplorationPercentageBy10
    {
        get => ExplorationPercentage / 10.0;
    }

    /// <summary>
    /// 类型名称转换器
    /// </summary>
    public string TypeFormatted
    {
        get => IsReputation ? "声望等级" : "供奉等级";
    }

    /// <summary>
    /// 指示当前是否为声望
    /// </summary>
    public bool IsReputation
    {
        get => Type == "Reputation";
    }
}

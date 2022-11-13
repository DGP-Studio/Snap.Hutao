// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

namespace Snap.Hutao.Web.Hoyolab.App.GameRecord;

/// <summary>
/// 小组件数据
/// </summary>
public class WidgetData
{
    /// <summary>
    /// 游戏Id 2
    /// </summary>
    [JsonPropertyName("game_id")]
    public int GameId { get; set; }

    /// <summary>
    /// 游戏内Uid
    /// </summary>
    [JsonPropertyName("game_role_id")]
    public string GameRoleId { get; set; } = default!;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = default!;

    /// <summary>
    /// 区服
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

    /// <summary>
    /// 冒险等阶
    /// </summary>
    [JsonPropertyName("level")]
    public int Level { get; set; }

    /// <summary>
    /// 背景图片
    /// </summary>
    [JsonPropertyName("background_image")]
    public string BackgroundImage { get; set; } = default!;

    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("data")]
    public List<WidgetDataItem> Data { get; set; } = default!;

    /// <summary>
    /// 区服名称
    /// </summary>
    [JsonPropertyName("region_name")]
    public string RegionName { get; set; } = default!;
}
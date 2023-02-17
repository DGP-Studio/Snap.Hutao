// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Widget;

/// <summary>
/// 小组件数据项
/// </summary>
[HighQuality]
internal sealed class WidgetDataItem
{
    /// <summary>
    /// 项目名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    /// <summary>
    /// 类型 均为 "String"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    /// <summary>
    /// 显示值
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = default!;
}
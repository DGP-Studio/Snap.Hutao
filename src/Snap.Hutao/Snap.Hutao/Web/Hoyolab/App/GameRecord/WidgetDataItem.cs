// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;

namespace Snap.Hutao.Web.Hoyolab.App.GameRecord;

/// <summary>
/// 小组件数据项
/// </summary>
public class WidgetDataItem
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
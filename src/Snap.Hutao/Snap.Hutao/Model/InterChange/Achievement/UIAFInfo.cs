// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Json.Serialization;

namespace Snap.Hutao.Model.InterChange.Achievement;

/// <summary>
/// UIAF格式的信息
/// </summary>
public class UIAFInfo
{
    /// <summary>
    /// 导出的 App 名称
    /// </summary>
    [JsonPropertyName("export_app")]
    public string ExportApp { get; set; } = default!;

    /// <summary>
    /// 导出的时间戳
    /// </summary>
    [JsonPropertyName("export_timestamp")]
    public long? ExportTimestamp { get; set; }

    /// <summary>
    /// 导出时间
    /// </summary>
    public DateTimeOffset ExportDateTime
    {
        get => DateTimeOffset.FromUnixTimeSeconds(ExportTimestamp ?? DateTimeOffset.MinValue.ToUnixTimeSeconds());
    }

    /// <summary>
    /// 导出的 App 版本
    /// </summary>
    [JsonPropertyName("export_app_version")]
    public string? ExportAppVersion { get; set; }

    /// <summary>
    /// 使用的UIAF版本
    /// </summary>
    [JsonPropertyName("uiaf_version")]
    public string? UIAFVersion { get; set; }
}

// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Model.InterChange.Achievement;

/// <summary>
/// UIAF格式的信息
/// </summary>
[HighQuality]
internal sealed class UIAFInfo
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
    [JsonIgnore]
    public DateTimeOffset ExportDateTime
    {
        get => DateTimeOffsetExtension.FromUnixTime(ExportTimestamp, DateTimeOffset.MinValue);
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

    /// <summary>
    /// 构造一个新的专用 UIAF 信息
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <returns>专用 UIAF 信息</returns>
    public static UIAFInfo Create(IServiceProvider serviceProvider)
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();

        return new()
        {
            ExportTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            ExportApp = SH.AppName,
            ExportAppVersion = hutaoOptions.Version.ToString(),
            UIAFVersion = UIAF.CurrentVersion,
        };
    }
}

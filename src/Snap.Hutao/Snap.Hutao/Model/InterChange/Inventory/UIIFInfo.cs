// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Extension;

namespace Snap.Hutao.Model.InterChange.Inventory;

/// <summary>
/// UIIF格式的信息
/// </summary>
[HighQuality]
internal sealed class UIIFInfo
{
    /// <summary>
    /// 用户Uid
    /// </summary>
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 语言
    /// </summary>
    [JsonPropertyName("lang")]
    public string Language { get; set; } = default!;

    /// <summary>
    /// 导出的时间戳
    /// </summary>
    [JsonPropertyName("export_timestamp")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
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
    /// 导出的 App 名称
    /// </summary>
    [JsonPropertyName("export_app")]
    public string ExportApp { get; set; } = default!;

    /// <summary>
    /// 导出的 App 版本
    /// </summary>
    [JsonPropertyName("export_app_version")]
    public string ExportAppVersion { get; set; } = default!;

    /// <summary>
    /// 使用的UIGF版本
    /// </summary>
    [JsonPropertyName("uiif_version")]
    public string UIIFVersion { get; set; } = default!;

    /// <summary>
    /// 构造一个新的专用 UIGF 信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>专用 UIGF 信息</returns>
    public static UIIFInfo Create(string uid)
    {
        return new()
        {
            Uid = uid,
            Language = "zh-cn",
            ExportTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            ExportApp = "胡桃",
            ExportAppVersion = CoreEnvironment.Version.ToString(),
            UIIFVersion = UIIF.CurrentVersion,
        };
    }
}

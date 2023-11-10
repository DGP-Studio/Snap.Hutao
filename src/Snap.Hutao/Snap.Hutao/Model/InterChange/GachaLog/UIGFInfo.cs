// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// UIGF格式的信息
/// </summary>
[HighQuality]
internal sealed class UIGFInfo : IMappingFrom<UIGFInfo, RuntimeOptions, MetadataOptions, string>
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
    [JsonPropertyName("uigf_version")]
    public string UIGFVersion { get; set; } = default!;

    /// <summary>
    /// 时区偏移
    /// </summary>
    [JsonPropertyName("region_time_zone")]
    public int? RegionTimeZone { get; set; } = default!;

    public static UIGFInfo From(RuntimeOptions runtimeOptions, MetadataOptions metadataOptions, string uid)
    {
        return new()
        {
            Uid = uid,
            Language = metadataOptions.LanguageCode,
            ExportTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
            ExportApp = SH.AppName,
            ExportAppVersion = runtimeOptions.Version.ToString(),
            UIGFVersion = UIGF.CurrentVersion,
            RegionTimeZone = PlayerUid.GetRegionTimeZoneUtcOffset(uid).Hours,
        };
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Model.InterChange.Achievement;

internal sealed class UIAFInfo
{
    [JsonPropertyName("export_app")]
    public string ExportApp { get; set; } = default!;

    [JsonPropertyName("export_timestamp")]
    public long? ExportTimestamp { get; set; }

    [JsonIgnore]
    public DateTimeOffset ExportDateTime
    {
        get => DateTimeOffsetExtension.UnsafeRelaxedFromUnixTime(ExportTimestamp, DateTimeOffset.MinValue);
    }

    [JsonPropertyName("export_app_version")]
    public string? ExportAppVersion { get; set; }

    [JsonPropertyName("uiaf_version")]
    public string? UIAFVersion { get; set; }

    public static UIAFInfo Create()
    {
        return new()
        {
            ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExportApp = SH.AppName,
            ExportAppVersion = HutaoRuntime.Version.ToString(),
            UIAFVersion = UIAF.CurrentVersion,
        };
    }
}

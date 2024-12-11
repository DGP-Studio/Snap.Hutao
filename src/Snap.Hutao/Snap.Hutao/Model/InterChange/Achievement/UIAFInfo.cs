// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using Snap.Hutao.Core;

namespace Snap.Hutao.Model.InterChange.Achievement;

// ReSharper disable once InconsistentNaming
internal sealed class UIAFInfo
{
    [JsonPropertyName("export_app")]
    public string? ExportApp { get; init; }

    [JsonPropertyName("export_timestamp")]
    public long? ExportTimestamp { get; init; }

    [JsonIgnore]
    [UsedImplicitly]
    public DateTimeOffset ExportDateTime
    {
        get => DateTimeOffsetExtension.UnsafeRelaxedFromUnixTime(ExportTimestamp, DateTimeOffset.MinValue);
    }

    [JsonPropertyName("export_app_version")]
    public string? ExportAppVersion { get; init; }

    // ReSharper disable once InconsistentNaming
    [JsonPropertyName("uiaf_version")]
    public string? UIAFVersion { get; init; }

    public static UIAFInfo CreateForExport()
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
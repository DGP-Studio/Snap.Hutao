// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Model.InterChange.Inventory;

// ReSharper disable once InconsistentNaming
internal sealed class UIIFInfo
{
    [JsonPropertyName("export_timestamp")]
    public long? ExportTimestamp { get; init; }

    [JsonIgnore]
    public DateTimeOffset ExportDateTime
    {
        get => DateTimeOffsetExtension.UnsafeRelaxedFromUnixTime(ExportTimestamp, DateTimeOffset.MinValue);
    }

    [JsonPropertyName("export_app")]
    public string? ExportApp { get; init; }

    [JsonPropertyName("export_app_version")]
    public string? ExportAppVersion { get; init; }

    // ReSharper disable once InconsistentNaming
    [JsonPropertyName("uiif_version")]
    public string? UIIFVersion { get; init; }

    public static UIIFInfo CreateForExport()
    {
        return new()
        {
            ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExportApp = SH.AppName,
            ExportAppVersion = HutaoRuntime.Version.ToString(),
            UIIFVersion = UIIF.CurrentVersion,
        };
    }

    public static UIIFInfo CreateForEmbeddedYae()
    {
        return new()
        {
            ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExportApp = "Embedded Yae",
            UIIFVersion = UIIF.CurrentVersion,
        };
    }
}
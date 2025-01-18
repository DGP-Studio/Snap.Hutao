// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Model.InterChange.Inventory;

internal sealed class UIIFInfo
{
    [JsonPropertyName("export_timestamp")]
    public long? ExportTimestamp { get; set; }

    [JsonIgnore]
    public DateTimeOffset ExportDateTime
    {
        get => DateTimeOffsetExtension.UnsafeRelaxedFromUnixTime(ExportTimestamp, DateTimeOffset.MinValue);
    }

    [JsonPropertyName("export_app")]
    public string ExportApp { get; set; } = default!;

    [JsonPropertyName("export_app_version")]
    public string ExportAppVersion { get; set; } = default!;

    [JsonPropertyName("uiif_version")]
    public string UIIFVersion { get; set; } = default!;

    public static UIIFInfo From(string uid)
    {
        return new()
        {
            ExportTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ExportApp = SH.AppName,
            ExportAppVersion = HutaoRuntime.Version.ToString(),
            UIIFVersion = UIIF.CurrentVersion,
        };
    }
}
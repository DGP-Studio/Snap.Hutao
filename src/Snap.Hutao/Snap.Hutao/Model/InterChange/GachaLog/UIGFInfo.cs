// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

// ReSharper disable once InconsistentNaming
internal sealed class UIGFInfo
{
    [JsonPropertyName("export_timestamp")]
    public required long ExportTimestamp { get; init; }

    [JsonPropertyName("export_app")]
    public required string ExportApp { get; init; }

    [JsonPropertyName("export_app_version")]
    public required string ExportAppVersion { get; init; }

    [JsonPropertyName("version")]
    public required string Version { get; init; }
}
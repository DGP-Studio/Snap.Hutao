// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class UIGFInfo
{
    [JsonPropertyName("export_timestamp")]
    public required long ExportTimestamp { get; set; }

    [JsonPropertyName("export_app")]
    public required string ExportApp { get; set; }

    [JsonPropertyName("export_app_version")]
    public required string ExportAppVersion { get; set; }

    [JsonPropertyName("version")]
    public required string Version { get; set; }
}
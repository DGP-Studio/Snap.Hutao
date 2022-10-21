// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

[SuppressMessage("", "SA1600")]
public abstract class Log
{
    [JsonIgnore]
    public LogStatus Status { get; set; } = LogStatus.Pending;

    [JsonPropertyName("type")]
    public abstract string Type { get; }

    [JsonPropertyName("sid")]
    public Guid Session { get; set; }

    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ");

    [JsonPropertyName("device")]
    public Device Device { get; set; } = default!;
}
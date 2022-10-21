// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model;

[SuppressMessage("", "SA1600")]
public class LogUploadResult
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = null!;

    [JsonPropertyName("validDiagnosticsIds")]
    public List<Guid> ValidDiagnosticsIds { get; set; } = null!;

    [JsonPropertyName("throttledDiagnosticsIds")]
    public List<Guid> ThrottledDiagnosticsIds { get; set; } = null!;

    [JsonPropertyName("correlationId")]
    public Guid CorrelationId { get; set; }
}
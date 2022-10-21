// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model;

[SuppressMessage("", "SA1600")]
public class AppCenterException
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "UnknownType";

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("stackTrace")]
    public string? StackTrace { get; set; }

    [JsonPropertyName("innerExceptions")]
    public List<AppCenterException>? InnerExceptions { get; set; }
}
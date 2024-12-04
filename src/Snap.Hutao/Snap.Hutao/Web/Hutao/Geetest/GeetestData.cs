// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Geetest;

internal sealed class GeetestData
{
    [JsonPropertyName("gt")]
    public string Gt { get; set; } = default!;

    [JsonPropertyName("challenge")]
    public string Challenge { get; set; } = default!;

    [JsonPropertyName("validate")]
    public string Validate { get; set; } = default!;

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;
}
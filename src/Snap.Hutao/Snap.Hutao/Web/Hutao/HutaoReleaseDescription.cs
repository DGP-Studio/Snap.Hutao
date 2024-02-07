// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class HutaoReleaseDescription
{
    [JsonPropertyName("cn")]
    public string CN { get; set; } = default!;

    [JsonPropertyName("en")]
    public string EN { get; set; } = default!;

    [JsonPropertyName("full")]
    public string Full { get; set; } = default!;
}
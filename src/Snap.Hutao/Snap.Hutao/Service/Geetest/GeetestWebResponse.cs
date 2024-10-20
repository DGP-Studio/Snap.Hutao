// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Geetest;

internal sealed class GeetestWebResponse
{
    [JsonPropertyName("geetest_challenge")]
    public string Challenge { get; set; } = default!;

    [JsonPropertyName("geetest_validate")]
    public string Validate { get; set; } = default!;

    [JsonPropertyName("geetest_seccode")]
    public string Seccode { get; set; } = default!;
}
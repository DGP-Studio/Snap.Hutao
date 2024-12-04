// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Geetest;

internal sealed class GeetestResponse
{
    public static GeetestResponse InternalFailure { get; } = new() { Code = Web.Response.Response.InternalFailure };

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("info")]
    public string Info { get; set; } = default!;

    [JsonPropertyName("data")]
    public GeetestData Data { get; set; } = default!;

    [JsonPropertyName("times")]
    public int Times { get; set; } = default!;
}
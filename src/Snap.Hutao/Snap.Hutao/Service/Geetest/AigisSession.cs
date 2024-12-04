// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Geetest;

internal sealed class AigisSession
{
    [JsonPropertyName("session_id")]
    public string SessionId { get; set; } = default!;

    [JsonPropertyName("mmt_type")]
    public int MmtType { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; } = default!;
}
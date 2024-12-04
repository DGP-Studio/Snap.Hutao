// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;

internal sealed class GameLoginResult
{
    [JsonPropertyName("stat")]
    public string Stat { get; set; } = default!;

    [JsonPropertyName("payload")]
    public GameLoginResultPayload Payload { get; set; } = default!;
}

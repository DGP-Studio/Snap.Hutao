// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Sdk.Combo;

internal sealed class GameLoginResultPayload
{
    [JsonPropertyName("proto")]
    public string Proto { get; set; } = default!;

    [JsonPropertyName("raw")]
    public string Raw { get; set; } = default!;

    [JsonPropertyName("ext")]
    public string Ext { get; set; } = default!;
}

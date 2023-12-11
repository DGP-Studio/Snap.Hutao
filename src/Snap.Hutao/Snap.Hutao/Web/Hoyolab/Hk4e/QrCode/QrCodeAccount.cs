// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

[HighQuality]
internal sealed class QrCodeAccount
{
    [JsonPropertyName("uid")]
    public string Stuid { get; set; } = default!;

    [JsonPropertyName("token")]
    public string GameToken { get; set; } = default!;
}

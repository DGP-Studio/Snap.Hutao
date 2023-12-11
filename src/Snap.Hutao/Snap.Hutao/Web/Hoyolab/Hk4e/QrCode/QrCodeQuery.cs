// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

[HighQuality]
internal sealed class QrCodeQuery
{
    [JsonPropertyName("stat")]
    public string Stat { get; set; } = default!;

    [JsonPropertyName("payload")]
    public QrCodeQueryPayload Payload { get; set; } = default!;
}

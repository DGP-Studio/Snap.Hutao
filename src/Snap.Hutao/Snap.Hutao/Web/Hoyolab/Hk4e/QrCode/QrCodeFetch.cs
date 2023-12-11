// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

[HighQuality]
internal sealed class QrCodeFetch
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}

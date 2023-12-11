// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

[HighQuality]
internal sealed class QrCodeQueryOptions
{
    public QrCodeQueryOptions(int appId, string device, string ticket)
    {
        AppId = appId;
        Device = device;
        Ticket = ticket;
    }

    [JsonPropertyName("app_id")]
    public int AppId { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = default!;

    [JsonPropertyName("ticket")]
    public string Ticket { get; set; } = default!;
}

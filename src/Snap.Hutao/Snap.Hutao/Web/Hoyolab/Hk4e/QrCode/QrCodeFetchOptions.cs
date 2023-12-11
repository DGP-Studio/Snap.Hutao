// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Hk4e.QrCode;

[HighQuality]
internal sealed class QrCodeFetchOptions
{
    public QrCodeFetchOptions(int appId, string device)
    {
        AppId = appId;
        Device = device;
    }

    [JsonPropertyName("app_id")]
    public int AppId { get; set; }

    [JsonPropertyName("device")]
    public string Device { get; set; } = default!;
}

// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

[SuppressMessage("", "SA1124")]
internal sealed class ShareContent
{
    #region Screenshot

    [JsonPropertyName("preview")]
    public bool Preview { get; set; }

    #endregion

    #region Image

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("image_base64")]
    public string? ImageBase64 { get; set; }

    #endregion
}

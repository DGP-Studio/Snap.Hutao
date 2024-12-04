// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

internal sealed class GameAuthKey
{
    [JsonPropertyName("authkey")]
    public string AuthKey { get; set; } = default!;

    [JsonPropertyName("authkey_ver")]
    public int AuthKeyVersion { get; set; } = default!;

    [JsonPropertyName("sign_type")]
    public int SignType { get; set; } = default!;
}

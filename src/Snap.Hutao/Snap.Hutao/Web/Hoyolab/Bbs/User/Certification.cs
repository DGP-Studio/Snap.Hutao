// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal class Certification
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; } = default!;
}

// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher.Content;

internal sealed class Link
{
    [JsonPropertyName("faq")]
    public string FAQ { get; set; } = default!;

    [JsonPropertyName("version")]
    public string Version { get; set; } = default!;
}
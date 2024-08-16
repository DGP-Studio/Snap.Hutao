// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class HomeBanner
{
    [JsonPropertyName("cover")]
    public string Cover { get; set; } = default!;

    [JsonPropertyName("app_path")]
    public string AppPath { get; set; } = default!;
}
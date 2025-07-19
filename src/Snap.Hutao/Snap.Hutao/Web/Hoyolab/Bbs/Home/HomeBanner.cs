// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal sealed class HomeBanner
{
    [JsonPropertyName("cover")]
    public required Uri Cover { get; init; }

    [JsonPropertyName("app_path")]
    public required Uri AppPath { get; init; }
}
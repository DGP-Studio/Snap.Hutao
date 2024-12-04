// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

internal sealed class PrivacyInvisible
{
    [JsonPropertyName("post")]
    public bool Post { get; set; }

    [JsonPropertyName("collect")]
    public bool Collect { get; set; }

    [JsonPropertyName("watermark")]
    public bool Watermark { get; set; }

    [JsonPropertyName("reply")]
    public bool Reply { get; set; }

    [JsonPropertyName("post_and_instant")]
    public bool PostAndInstant { get; set; }
}

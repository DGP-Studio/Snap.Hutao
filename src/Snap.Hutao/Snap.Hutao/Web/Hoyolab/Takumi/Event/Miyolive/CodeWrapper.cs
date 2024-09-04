// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

internal sealed class CodeWrapper
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("code")]
    public string Code { get; set; } = default!;

    [JsonPropertyName("img")]
    public string Image { get; set; } = default!;

    [JsonPropertyName("to_get_time")]
    public long ToGetTime { get; set; } = default!;
}
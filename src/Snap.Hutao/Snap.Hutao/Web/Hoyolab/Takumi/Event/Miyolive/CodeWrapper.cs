// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

internal sealed class CodeWrapper
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("img")]
    public required string Image { get; init; }

    [JsonPropertyName("to_get_time")]
    public required long ToGetTime { get; init; }

    public CodeWrapper WithTitle(string title)
    {
        return new()
        {
            Title = title,
            Code = Code,
            Image = Image,
            ToGetTime = ToGetTime,
        };
    }
}
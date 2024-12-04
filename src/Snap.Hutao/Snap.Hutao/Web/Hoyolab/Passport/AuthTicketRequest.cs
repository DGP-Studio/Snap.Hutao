// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class AuthTicketRequest
{
    [JsonPropertyName("game_biz")]
    public string GameBiz { get; set; } = default!;

    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    [JsonPropertyName("stoken")]
    public string SToken { get; set; } = default!;

    [JsonPropertyName("uid")]
    public int Uid { get; set; }
}
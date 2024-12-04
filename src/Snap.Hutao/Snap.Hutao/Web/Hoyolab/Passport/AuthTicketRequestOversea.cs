// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class AuthTicketRequestOversea
{
    [JsonPropertyName("biz_name")]
    public string BizName { get; set; } = default!;

    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    [JsonPropertyName("stoken")]
    public string SToken { get; set; } = default!;
}
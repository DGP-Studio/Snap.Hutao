// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal class LoginResult
{
    [JsonPropertyName("token")]
    public TokenWrapper? Token { get; set; } = default!;

    [JsonPropertyName("user_info")]
    public UserInformation? UserInfo { get; set; } = default!;

    [JsonPropertyName("reactivate_info")]
    public ReactivateInfo ReactivateInfo { get; set; } = default!;

    [JsonPropertyName("login_ticket")]
    public string LoginTicket { get; set; } = default!;
}
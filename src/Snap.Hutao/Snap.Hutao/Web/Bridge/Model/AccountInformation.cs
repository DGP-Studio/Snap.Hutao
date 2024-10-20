// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Passport;

namespace Snap.Hutao.Web.Bridge.Model;

internal sealed class AccountInformation
{
    public AccountInformation(UserInformation userInformation, string ltoken)
    {
        Aid = userInformation.Aid;
        Mid = userInformation.Mid;
        AccountName = userInformation.AccountName;
        AreaCode = userInformation.AreaCode;
        Mobile = userInformation.Mobile;
        Email = userInformation.Email;
        Token = ltoken;
        TokenType = 2;
    }

    [JsonPropertyName("aid")]
    public string Aid { get; set; } = default!;

    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    [JsonPropertyName("accountName")]
    public string AccountName { get; set; } = default!;

    [JsonPropertyName("areaCode")]
    public string AreaCode { get; set; } = default!;

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("tokenStr")]
    public string Token { get; set; } = default!;

    [JsonPropertyName("tokenType")]
    public int TokenType { get; set; }
}
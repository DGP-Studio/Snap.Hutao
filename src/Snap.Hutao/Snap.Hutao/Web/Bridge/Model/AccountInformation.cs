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
    public string Aid { get; set; }

    [JsonPropertyName("mid")]
    public string Mid { get; set; }

    [JsonPropertyName("accountName")]
    public string AccountName { get; set; }

    [JsonPropertyName("areaCode")]
    public string AreaCode { get; set; }

    [JsonPropertyName("mobile")]
    public string Mobile { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("tokenStr")]
    public string Token { get; set; }

    [JsonPropertyName("tokenType")]
    public int TokenType { get; set; }
}
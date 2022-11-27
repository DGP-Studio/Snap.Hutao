// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Passport;

namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// 账户信息
/// </summary>
public class AccountInformation
{
    /// <summary>
    /// 构造一个新的账户信息
    /// </summary>
    /// <param name="userInformation">用户信息</param>
    /// <param name="ltoken">ltoken</param>
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

    /// <summary>
    /// 账户Id
    /// </summary>
    [JsonPropertyName("aid")]
    public string Aid { get; set; } = default!;

    /// <summary>
    /// 米哈游Id
    /// </summary>
    [JsonPropertyName("mid")]
    public string Mid { get; set; } = default!;

    /// <summary>
    /// 空
    /// </summary>
    [JsonPropertyName("accountName")]
    public string AccountName { get; set; } = default!;

    /// <summary>
    /// 区域码 +86
    /// </summary>
    [JsonPropertyName("areaCode")]
    public string AreaCode { get; set; } = default!;

    /// <summary>
    /// 手机号 111****1111
    /// </summary>
    [JsonPropertyName("mobile")]
    public string Mobile { get; set; } = default!;

    /// <summary>
    /// 空
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    /// <summary>
    /// Token
    /// </summary>
    [JsonPropertyName("tokenStr")]
    public string Token { get; set; } = default!;

    /// <summary>
    /// Token 类型
    /// </summary>
    [JsonPropertyName("tokenType")]
    public int TokenType { get; set; } = default!;
}
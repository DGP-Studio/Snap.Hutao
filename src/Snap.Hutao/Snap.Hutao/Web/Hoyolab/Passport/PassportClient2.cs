// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端 XRPC 版
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class PassportClient2
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<PassportClient> logger;

    /// <summary>
    /// 构造一个新的通行证客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public PassportClient2(HttpClient httpClient, JsonSerializerOptions options, ILogger<PassportClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步账密登录
    /// </summary>
    /// <param name="account">用户</param>
    /// <param name="password">密码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>登录数据</returns>
    public async Task<LoginResult?> LoginByPasswordAsync(string account, string password, CancellationToken token)
    {
        Dictionary<string, string> data = new()
        {
            { "account", RSAEncryptedString.Encrypt(account) },
            { "password", RSAEncryptedString.Encrypt(password) },
        };

        Response<LoginResult>? resp = await httpClient
            .SetAppId()
            .SetHeader("x-rpc-aigis", string.Empty)
            .SetHeader("x-rpc-client_type", "2")
            .SetHeader("x-rpc-game_biz", "bbs_cn")
            .SetHeader("x-rpc-sdk_version", "1.3.1.2")
            .UsingDynamicSecret2(SaltType.PROD, options, ApiEndpoints.AccountLoginByPassword, data)
            .TryCatchPostAsJsonAsync<Response<LoginResult>>(logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>uid 与 cookie token</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.PROD)]
    public async Task<UidCookieToken?> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token)
    {
        Response<UidCookieToken>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UsingDynamicSecret2(SaltType.PROD, options, ApiEndpoints.AccountCookieAccountInfoBySToken)
            .TryCatchGetFromJsonAsync<Response<UidCookieToken>>(logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }
}

public class LoginResult
{
    [JsonPropertyName("token")]
    public TokenWrapper Token { get; set; }

    [JsonPropertyName("user_info")]
    public UserInformation UserInfo { get; set; }

    [JsonPropertyName("reactivate_info")]
    public ReactivateInfo ReactivateInfo { get; set; }

    [JsonPropertyName("login_ticket")]
    public string LoginTicket { get; set; }
}

public class ReactivateInfo
{

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("ticket")]
    public string Ticket { get; set; }
}

public class TokenWrapper
{
    [JsonPropertyName("token_type")]
    public int TokenType { get; set; }

    [JsonPropertyName("token")]
    public string Token { get; set; }
}
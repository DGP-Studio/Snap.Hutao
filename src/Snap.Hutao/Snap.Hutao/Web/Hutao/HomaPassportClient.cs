// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃通行证客户端
/// </summary>
[HighQuality]
[HttpClient(HttpClientConfiguration.Default)]
[ConstructorGenerated(ResolveHttpClient = true)]
internal sealed partial class HomaPassportClient
{
    /// <summary>
    /// 通行证请求公钥
    /// </summary>
    public const string PublicKey = """
        -----BEGIN PUBLIC KEY-----
        MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA5W2SEyZSlP2zBI1Sn8Gd
        TwbZoXlUGNKyoVrY8SVYu9GMefdGZCrUQNkCG/Np8pWPmSSEFGd5oeug/oIMtCZQ
        NOn0drlR+pul/XZ1KQhKmj/arWjN1XNok2qXF7uxhqD0JyNT/Fxy6QvzqIpBsM9S
        7ajm8/BOGlPG1SInDPaqTdTRTT30AuN+IhWEEFwT3Ctv1SmDupHs2Oan5qM7Y3uw
        b6K1rbnk5YokiV2FzHajGUymmSKXqtG1USZzwPqImpYb4Z0M/StPFWdsKqexBqMM
        mkXckI5O98GdlszEmQ0Ejv5Fx9fR2rXRwM76S4iZTfabYpiMbb4bM42mHMauupj6
        9QIDAQAB
        -----END PUBLIC KEY-----
        """;

    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaPassportClient> logger;

    /// <summary>
    /// 异步获取验证码
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="isResetPassword">是否重置账号密码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    public async ValueTask<Response.Response> VerifyAsync(string email, bool isResetPassword, CancellationToken token = default)
    {
        Dictionary<string, object> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["IsResetPassword"] = isResetPassword,
        };

        Response.Response? resp = await httpClient
            .TryCatchPostAsJsonAsync<Dictionary<string, object>, Response.Response>(HutaoEndpoints.PassportVerify, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步注册
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="password">密码</param>
    /// <param name="verifyCode">验证码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应，包含登录令牌</returns>
    public async ValueTask<Response<string>> RegisterAsync(string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        Response<string>? resp = await httpClient
            .TryCatchPostAsJsonAsync<Dictionary<string, string>, Response<string>>(HutaoEndpoints.PassportRegister, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步重置密码
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="password">密码</param>
    /// <param name="verifyCode">验证码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应，包含登录令牌</returns>
    public async ValueTask<Response<string>> ResetPasswordAsync(string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        Response<string>? resp = await httpClient
            .TryCatchPostAsJsonAsync<Dictionary<string, string>, Response<string>>(HutaoEndpoints.PassportResetPassword, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步登录
    /// </summary>
    /// <param name="email">邮箱</param>
    /// <param name="password">密码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应，包含登录令牌</returns>
    public async ValueTask<Response<string>> LoginAsync(string email, string password, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
        };

        Response<string>? resp = await httpClient
            .TryCatchPostAsJsonAsync<Dictionary<string, string>, Response<string>>(HutaoEndpoints.PassportLogin, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="authToken">验证令牌</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户信息</returns>
    public async ValueTask<Response<UserInfo>> GetUserInfoAsync(string authToken, CancellationToken token = default)
    {
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", authToken);

        Response<UserInfo>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<UserInfo>>(HutaoEndpoints.PassportUserInfo, options, logger, token)
            .ConfigureAwait(false);

        httpClient.DefaultRequestHeaders.Authorization = default;

        return Response.Response.DefaultIfNull(resp);
    }

    private static string Encrypt(string text)
    {
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(text);
        using (RSA rsa = RSA.Create(2048))
        {
            rsa.ImportFromPem(PublicKey);
            byte[] encryptedBytes = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA1);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
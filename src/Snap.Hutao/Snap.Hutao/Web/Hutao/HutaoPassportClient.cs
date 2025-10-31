// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Web.Hutao;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoPassportClient
{
    private const string PublicKey = """
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

    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HutaoPassportClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<HutaoResponse> RequestVerifyAsync(string email, VerifyCodeRequestType requestType, CancellationToken token = default)
    {
        Dictionary<string, object> data = new()
        {
            ["UserName"] = Encrypt(email),
        };

        if (requestType.HasFlag(VerifyCodeRequestType.ResetPassword))
        {
            data["IsResetPassword"] = true;
        }

        if (requestType.HasFlag(VerifyCodeRequestType.CancelRegistration))
        {
            data["IsCancelRegistration"] = true;
        }

        if (requestType.HasFlag(VerifyCodeRequestType.ResetUserName))
        {
            data["IsResetUserName"] = true;
        }

        if (requestType.HasFlag(VerifyCodeRequestType.ResetUserNameNew))
        {
            data["IsResetUserNameNew"] = true;
        }

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportVerify())
            .PostJson(data);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<TokenSet>> RegisterAsync(string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportRegister())
            .PostJson(data);

        HutaoResponse<TokenSet>? resp = await builder
            .SendAsync<HutaoResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> UnregisterAsync(string? accessToken, string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportCancel())
            .SetAccessToken(accessToken)
            .PostJson(data);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<TokenSet>> ResetUserNameAsync(string email, string newUserName, string verifyCode, string newVerifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["NewUserName"] = Encrypt(newUserName),
            ["VerifyCode"] = Encrypt(verifyCode),
            ["NewVerifyCode"] = Encrypt(newVerifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportResetUserName())
            .PostJson(data);

        HutaoResponse<TokenSet>? resp = await builder
            .SendAsync<HutaoResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<TokenSet>> ResetPasswordAsync(string email, string password, string verifyCode, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
            ["VerifyCode"] = Encrypt(verifyCode),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportResetPassword())
            .PostJson(data);

        HutaoResponse<TokenSet>? resp = await builder
            .SendAsync<HutaoResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<TokenSet>> LoginAsync(string email, string password, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["UserName"] = Encrypt(email),
            ["Password"] = Encrypt(password),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportLogin())
            .PostJson(data);

        HutaoResponse<TokenSet>? resp = await builder
            .SendAsync<HutaoResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<UserInfo>> GetUserInfoAsync(string? accessToken, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportUserInfo())
            .SetAccessToken(accessToken)
            .Get();

        HutaoResponse<UserInfo>? resp = await builder
            .SendAsync<HutaoResponse<UserInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<TokenSet>> RefreshTokenAsync(string refreshToken, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["RefreshToken"] = Encrypt(refreshToken),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportRefreshToken())
            .PostJson(data);

        HutaoResponse<TokenSet>? resp = await builder
            .SendAsync<HutaoResponse<TokenSet>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> RevokeTokenAsync(string? accessToken, string deviceId, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["DeviceId"] = Encrypt(deviceId),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportRevokeToken())
            .SetAccessToken(accessToken)
            .PostJson(data);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> RevokeAllTokensAsync(string? accessToken, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().PassportRevokeAllTokens())
            .SetAccessToken(accessToken)
            .Post();

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    private static string Encrypt(string text)
    {
        using (RSA rsa = RSA.Create(2048))
        {
            rsa.ImportFromPem(PublicKey);
            byte[] encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.OaepSHA1);
            return Convert.ToBase64String(encryptedBytes);
        }
    }
}
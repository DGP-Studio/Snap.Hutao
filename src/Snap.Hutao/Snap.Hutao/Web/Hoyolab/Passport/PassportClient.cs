// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[HttpClient(HttpClientConfiguration.XRpc2)]
internal sealed partial class PassportClient : IPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial PassportClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetCookieTokenBySToken())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<UidCookieToken>? resp = await builder
            .SendAsync<Response<UidCookieToken>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetLTokenBySToken())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<LTokenWrapper>? resp = await builder
            .SendAsync<Response<LTokenWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<UserInfoWrapper>> VerifyLtokenAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountVerifyLtoken())
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .PostJson(new Timestamp());

        Response<UserInfoWrapper>? resp = await builder
            .SendAsync<Response<UserInfoWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LoginResult>> LoginBySTokenAsync(Cookie stokenV1, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetSTokenByOldToken())
            .SetHeader("Cookie", stokenV1.ToString())
            .Post();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<LoginResult>? resp = await builder
            .SendAsync<Response<LoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LoginResult>> LoginByGameTokenAsync(UidGameToken account, CancellationToken token = default)
    {
        AccountIdGameToken data = new()
        {
            AccountId = int.Parse(account.Uid, CultureInfo.InvariantCulture),
            GameToken = account.GameToken,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetSTokenByGameToken())
            .PostJson(data);

        Response<LoginResult>? resp = await builder
            .SendAsync<Response<LoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<(string? Aigis, Response<MobileCaptcha> Response)> CreateLoginCaptchaAsync(string mobile, string? aigis, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["area_code"] = Encrypt("+86"),
            ["mobile"] = Encrypt(mobile),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateLoginCaptcha())
            .PostJson(data);

        if (!string.IsNullOrEmpty(aigis))
        {
            builder.SetXrpcAigis(aigis);
        }

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        (HttpResponseHeaders? headers, Response<MobileCaptcha>? resp) = await builder
            .SendAsync<Response<MobileCaptcha>>(httpClient, token)
            .ConfigureAwait(false);

        string? rpcAigis = headers?.GetValuesOrDefault("X-Rpc-Aigis")?.SingleOrDefault();
        return (rpcAigis, Response.Response.DefaultIfNull(resp));
    }

    public ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(IPassportMobileCaptchaProvider provider, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(provider.ActionType);
        ArgumentNullException.ThrowIfNull(provider.Mobile);
        ArgumentNullException.ThrowIfNull(provider.Captcha);

        return LoginByMobileCaptchaAsync(provider.ActionType, provider.Mobile, provider.Captcha, provider.Aigis, token);
    }

    public async ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(string actionType, string mobile, string captcha, string? aigis, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["area_code"] = Encrypt("+86"),
            ["action_type"] = actionType,
            ["captcha"] = captcha,
            ["mobile"] = Encrypt(mobile),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountLoginByMobileCaptcha())
            .PostJson(data);

        if (!string.IsNullOrEmpty(aigis))
        {
            builder.SetXrpcAigis(aigis);
        }

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.PROD, true).ConfigureAwait(false);

        Response<LoginResult>? resp = await builder
            .SendAsync<Response<LoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public ValueTask<Response<ActionTicketInfo>> GetActionTicketInfoAsync(string ticket, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<ActionTicketInfo>>(new NotSupportedException());
    }

    public ValueTask<Response.Response> VerifyActionTicketPartlyAsync(string ticket, string captcha, CancellationToken token = default)
    {
        return ValueTask.FromException<Response.Response>(new NotSupportedException());
    }

    public ValueTask<(string? Aigis, Response.Response Response)> CreateEmailCaptchaByActionTicketAsync(string ticket, string? aigis, CancellationToken token = default)
    {
        return ValueTask.FromException<(string? Aigis, Response.Response Response)>(new NotSupportedException());
    }

    private static string Encrypt(string source)
    {
        using (RSA rsa = RSA.Create())
        {
            // https://www.rfc-editor.org/rfc/rfc7468#section-3
            // https://github.com/UIGF-org/Hoyolab.Salt#cn-passport-api-rsa_public_key
            rsa.ImportFromPem("""
                -----BEGIN PUBLIC KEY-----
                MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDvekdPMHN3AYhm/vktJT+YJr7
                cI5DcsNKqdsx5DZX0gDuWFuIjzdwButrIYPNmRJ1G8ybDIF7oDW2eEpm5sMbL9zs
                9ExXCdvqrn51qELbqj0XxtMTIpaCHFSI50PfPpTFV9Xt/hmyVwokoOXFlAEgCn+Q
                CgGs52bFoYMtyi+xEQIDAQAB
                -----END PUBLIC KEY-----
                """);
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(source), RSAEncryptionPadding.Pkcs1));
        }
    }

    private class Timestamp
    {
        [JsonPropertyName("t")]
        public long Time { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
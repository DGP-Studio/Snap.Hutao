// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[HttpClient(HttpClientConfiguration.XRpc6)]
internal sealed partial class HoyoPlayPassportClientOversea : IHoyoPlayPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HoyoPlayPassportClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default)
    {
        string? sToken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(sToken);
        ArgumentException.ThrowIfNullOrEmpty(user.Mid);

        AuthTicketRequestOversea data = new()
        {
            BizName = "hk4e_global",
            Mid = user.Mid,
            SToken = sToken,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateAuthTicketByGameBiz())
            .PostJson(data);

        Response<AuthTicketWrapper>? resp = await builder
            .SendAsync<Response<AuthTicketWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public ValueTask<Response<QrLogin>> CreateQrLoginAsync(CancellationToken token = default)
    {
        return ValueTask.FromException<Response<QrLogin>>(new NotSupportedException());
    }

    public ValueTask<Response<QrLoginResult>> QueryQrLoginStatusAsync(string ticket, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<QrLoginResult>>(new NotSupportedException());
    }

    public ValueTask<(string? Aigis, string? Risk, Response<LoginResult> Response)> LoginByPasswordAsync(IPassportPasswordProvider provider, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(provider.Account);
        ArgumentNullException.ThrowIfNull(provider.Password);

        return LoginByPasswordAsync(provider.Account, provider.Password, provider.Aigis, provider.Verify, token);
    }

    public async ValueTask<(string? Aigis, string? Risk, Response<LoginResult> Response)> LoginByPasswordAsync(string account, string password, string? aigis, string? verify, CancellationToken token = default)
    {
        Dictionary<string, string> data = new()
        {
            ["account"] = Encrypt(account),
            ["password"] = Encrypt(password),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountLoginByPassword())
            .PostJson(data);

        if (!string.IsNullOrEmpty(aigis))
        {
            builder.SetXrpcAigis(aigis);
        }

        if (!string.IsNullOrEmpty(verify))
        {
            builder.SetXrpcVerify(verify);
        }

        (HttpResponseHeaders? headers, Response<LoginResult>? resp) = await builder
            .SendAsync<Response<LoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        string? rpcAigis = headers?.GetValuesOrDefault("X-Rpc-Aigis")?.SingleOrDefault();
        string? rpcVerify = headers?.GetValuesOrDefault("X-Rpc-Verify")?.SingleOrDefault();
        return (rpcAigis, rpcVerify, Response.Response.DefaultIfNull(resp));
    }

    public async ValueTask<(string? Risk, Response<LoginResult> Response)> LoginByThirdPartyAsync(ThirdPartyToken thirdPartyToken, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountLoginByThirdParty())
            .PostJson(thirdPartyToken);

        (HttpResponseHeaders? headers, Response<LoginResult>? resp) = await builder
            .SendAsync<Response<LoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        string? rpcVerify = headers?.GetValuesOrDefault("X-Rpc-Verify")?.SingleOrDefault();
        return (rpcVerify, Response.Response.DefaultIfNull(resp));
    }

    private static string Encrypt(string source)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.ImportFromPem($"""
                -----BEGIN PUBLIC KEY-----
                MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA4PMS2JVMwBsOIrYWRluY
                wEiFZL7Aphtm9z5Eu/anzJ09nB00uhW+ScrDWFECPwpQto/GlOJYCUwVM/raQpAj
                /xvcjK5tNVzzK94mhk+j9RiQ+aWHaTXmOgurhxSp3YbwlRDvOgcq5yPiTz0+kSeK
                ZJcGeJ95bvJ+hJ/UMP0Zx2qB5PElZmiKvfiNqVUk8A8oxLJdBB5eCpqWV6CUqDKQ
                KSQP4sM0mZvQ1Sr4UcACVcYgYnCbTZMWhJTWkrNXqI8TMomekgny3y+d6NX/cFa6
                6jozFIF4HCX5aW8bp8C8vq2tFvFbleQ/Q3CU56EWWKMrOcpmFtRmC18s9biZBVR/
                8QIDAQAB
                -----END PUBLIC KEY-----
                """);
            return Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(source), RSAEncryptionPadding.Pkcs1));
        }
    }
}
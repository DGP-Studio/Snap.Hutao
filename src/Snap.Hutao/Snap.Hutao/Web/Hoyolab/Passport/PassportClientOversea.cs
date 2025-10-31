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

namespace Snap.Hutao.Web.Hoyolab.Passport;

[HttpClient(HttpClientConfiguration.XRpc3)]
internal sealed partial class PassportClientOversea : IPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Oversea)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial PassportClientOversea(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        string? sToken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(sToken);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        STokenWrapper data = new(sToken, user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetCookieTokenBySToken())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .PostJson(data);

        Response<UidCookieToken>? resp = await builder
            .SendAsync<Response<UidCookieToken>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<LTokenWrapper>> GetLTokenBySTokenAsync(User user, CancellationToken token = default)
    {
        string? sToken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(sToken);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);
        STokenWrapper data = new(sToken, user.Aid);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetLTokenBySToken())
            .SetUserCookieAndFpHeader(user, CookieType.SToken)
            .PostJson(data);

        Response<LTokenWrapper>? resp = await builder
            .SendAsync<Response<LTokenWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public ValueTask<Response<UserInfoWrapper>> VerifyLtokenAsync(User user, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<UserInfoWrapper>>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginBySTokenAsync(Cookie stokenV1, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginByGameTokenAsync(UidGameToken account, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public ValueTask<(string? Aigis, Response<MobileCaptcha> Response)> CreateLoginCaptchaAsync(string mobile, string? aigis, CancellationToken token = default)
    {
        return ValueTask.FromException<(string? Aigis, Response<MobileCaptcha> Response)>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(IPassportMobileCaptchaProvider provider, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public ValueTask<Response<LoginResult>> LoginByMobileCaptchaAsync(string actionType, string mobile, string captcha, string? aigis, CancellationToken token = default)
    {
        return ValueTask.FromException<Response<LoginResult>>(new NotSupportedException());
    }

    public async ValueTask<Response<ActionTicketInfo>> GetActionTicketInfoAsync(string ticket, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ticket);

        ActionTicketInfoRequestOversea data = new()
        {
            ActionTicket = ticket,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountGetActionTicketInfo())
            .PostJson(data);

        Response<ActionTicketInfo>? resp = await builder
            .SendAsync<Response<ActionTicketInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response.Response> VerifyActionTicketPartlyAsync(string ticket, string captcha, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ticket);
        ArgumentException.ThrowIfNullOrEmpty(captcha);

        ActionTicketInfoRequestOversea data = new()
        {
            ActionTicket = ticket,
            EmailCaptcha = captcha,
            VerifyMethod = 2,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountVerifyActionTicketPartly())
            .PostJson(data);

        Response.Response? resp = await builder
            .SendAsync<Response.Response>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<(string? Aigis, Response.Response Response)> CreateEmailCaptchaByActionTicketAsync(string ticket, string? aigis, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(ticket);

        ActionTicketInfoRequestOversea data = new()
        {
            ActionTicket = ticket,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateEmailCaptchaByActionTicket())
            .PostJson(data);

        if (!string.IsNullOrEmpty(aigis))
        {
            builder.SetXrpcAigis(aigis);
        }

        (HttpResponseHeaders? headers, Response.Response? resp) = await builder
            .SendAsync<Response.Response>(httpClient, token)
            .ConfigureAwait(false);

        string? rpcAigis = headers?.GetValuesOrDefault("X-Rpc-Aigis")?.SingleOrDefault();
        return (rpcAigis, Response.Response.DefaultIfNull(resp));
    }
}
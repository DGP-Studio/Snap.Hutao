// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Globalization;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Passport;

[HttpClient(HttpClientConfiguration.XRpc5)]
internal sealed partial class HoyoPlayPassportClient : IHoyoPlayPassportClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HoyoPlayPassportClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<AuthTicketWrapper>> CreateAuthTicketAsync(User user, CancellationToken token = default)
    {
        string? stoken = user.SToken?.GetValueOrDefault(Cookie.STOKEN);
        ArgumentException.ThrowIfNullOrEmpty(stoken);
        ArgumentException.ThrowIfNullOrEmpty(user.Mid);
        ArgumentException.ThrowIfNullOrEmpty(user.Aid);

        AuthTicketRequest data = new()
        {
            GameBiz = "hk4e_cn",
            Mid = user.Mid,
            SToken = stoken,
            Uid = int.Parse(user.Aid, CultureInfo.InvariantCulture),
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateAuthTicketByGameBiz())
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .PostJson(data);

        Response<AuthTicketWrapper>? resp = await builder
            .SendAsync<Response<AuthTicketWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<QrLogin>> CreateQrLoginAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountCreateQrLogin())
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId53)
            .PostJson(default(EmptyContent));

        Response<QrLogin>? resp = await builder
            .SendAsync<Response<QrLogin>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<QrLoginResult>> QueryQrLoginStatusAsync(string ticket, CancellationToken token = default)
    {
        QrTicketWrapper data = new()
        {
            Ticket = ticket,
        };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.AccountQueryQrLoginStatus())
            .SetHeader("x-rpc-device_id", HoyolabOptions.DeviceId53)
            .PostJson(data);

        Response<QrLoginResult>? resp = await builder
            .SendAsync<Response<QrLoginResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public ValueTask<(string? Aigis, string? Risk, Response<LoginResult> Response)> LoginByPasswordAsync(IPassportPasswordProvider provider, CancellationToken token = default)
    {
        return ValueTask.FromException<(string? Aigis, string? Risk, Response<LoginResult> Response)>(new NotSupportedException());
    }

    public ValueTask<(string? Aigis, string? Risk, Response<LoginResult> Response)> LoginByPasswordAsync(string account, string password, string? aigis, string? verify, CancellationToken token = default)
    {
        return ValueTask.FromException<(string? Aigis, string? Risk, Response<LoginResult> Response)>(new NotSupportedException());
    }

    public ValueTask<(string? Risk, Response<LoginResult> Response)> LoginByThirdPartyAsync(ThirdPartyToken thirdPartyToken, CancellationToken token = default)
    {
        return ValueTask.FromException<(string? Risk, Response<LoginResult> Response)>(new NotSupportedException());
    }
}
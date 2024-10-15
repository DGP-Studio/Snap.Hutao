// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Auth;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Binding;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class BindingClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<BindingClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesOverseaAwareAsync(User user, CancellationToken token = default)
    {
        if (user.IsOversea)
        {
            return await GetUserGameRolesByCookieAsync(user, token).ConfigureAwait(false);
        }

        Response<ActionTicketWrapper> actionTicketResponse = await serviceProvider
            .GetRequiredService<AuthClient>()
            .GetActionTicketBySTokenAsync("game_role", user, token)
            .ConfigureAwait(false);

        if (actionTicketResponse.IsOk())
        {
            string actionTicket = actionTicketResponse.Data.Ticket;
            return await GetUserGameRolesByActionTicketAsync(actionTicket, user, token).ConfigureAwait(false);
        }

        return Response.Response.CloneReturnCodeAndMessage<ListWrapper<UserGameRole>, ActionTicketWrapper>(actionTicketResponse);
    }

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByActionTicketAsync(string actionTicket, User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(user.IsOversea).UserGameRolesByActionTicket(actionTicket))
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .SendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<UserGameRole>>> GetUserGameRolesByCookieAsync(User user, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpointsFactory.Create(user.IsOversea).UserGameRolesByCookie())
            .SetUserCookieAndFpHeader(user, CookieType.LToken)
            .Get();

        Response<ListWrapper<UserGameRole>>? resp = await builder
            .SendAsync<Response<ListWrapper<UserGameRole>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
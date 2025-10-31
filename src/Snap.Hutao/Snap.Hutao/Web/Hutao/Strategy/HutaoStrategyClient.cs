// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.Immutable;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Strategy;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoStrategyClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HutaoStrategyClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<HutaoResponse<ImmutableDictionary<AvatarId, Strategy>>> GetStrategyAllAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StrategyAll())
            .Get();

        HutaoResponse<ImmutableDictionary<AvatarId, Strategy>>? resp = await builder.SendAsync<HutaoResponse<ImmutableDictionary<AvatarId, Strategy>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<ImmutableDictionary<AvatarId, Strategy>>> GetStrategyItemAsync(AvatarId avatarId, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StrategyItem(avatarId))
            .Get();

        HutaoResponse<ImmutableDictionary<AvatarId, Strategy>>? resp = await builder.SendAsync<HutaoResponse<ImmutableDictionary<AvatarId, Strategy>>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}
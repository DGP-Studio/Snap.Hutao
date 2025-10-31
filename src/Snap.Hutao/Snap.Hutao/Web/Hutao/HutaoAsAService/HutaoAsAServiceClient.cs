// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hutao.Redeem;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Collections.Immutable;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.HutaoAsAService;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoAsAServiceClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly CultureOptions cultureOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HutaoAsAServiceClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<HutaoResponse<ImmutableArray<Announcement>>> GetAnnouncementListAsync(ImmutableArray<long> excludedIds, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().Announcement(cultureOptions.LocaleName))
            .PostJson(excludedIds);

        HutaoResponse<ImmutableArray<Announcement>>? resp = await builder
            .SendAsync<HutaoResponse<ImmutableArray<Announcement>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> UploadAnnouncementAsync(string? accessToken, UploadAnnouncement uploadAnnouncement, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().AnnouncementUpload())
            .SetAccessToken(accessToken)
            .PostJson(uploadAnnouncement);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> GachaLogCompensationAsync(string? accessToken, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().GachaLogCompensation(days))
            .SetAccessToken(accessToken)
            .Get();

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> GachaLogDesignationAsync(string? accessToken, string userName, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().GachaLogDesignation(userName, days))
            .SetAccessToken(accessToken)
            .Get();

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> CdnCompensationAsync(string? accessToken, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().CdnCompensation(days))
            .SetAccessToken(accessToken)
            .Get();

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse> CdnDesignationAsync(string? accessToken, string userName, int days, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().CdnDesignation(userName, days))
            .SetAccessToken(accessToken)
            .Get();

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<RedeemGenerateResult>> GenerateRedeemCodesAsync(string? accessToken, RedeemGenerateRequest request, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RedeemCodeGenerate())
            .SetAccessToken(accessToken)
            .PostJson(request);

        HutaoResponse<RedeemGenerateResult>? resp = await builder
            .SendAsync<HutaoResponse<RedeemGenerateResult>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game.Island;
using Snap.Hutao.Service.Yae.Achievement;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Service.Feature;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IFeatureService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class FeatureService : IFeatureService
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IMemoryCache memoryCache;

    public async ValueTask<IslandFeature?> GetIslandFeatureAsync(string tag, string identifier)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory
            .Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().Feature($"IslandFeature_{tag}"))
            .SetHeader("x-hutao-island-identifier", identifier)
            .Get();

        using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(FeatureService)))
        {
            return (await builder.SendAsync<IslandFeature>(httpClient, CancellationToken.None).ConfigureAwait(false)).Body;
        }
    }

    public async ValueTask<AchievementFieldId?> GetAchievementFieldIdAsync(string tag)
    {
        return await memoryCache.GetOrCreateAsync($"{nameof(FeatureService)}.AchievementFieldId", async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(6));
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory
                .Create()
                .SetRequestUri(hutaoEndpointsFactory.Create().Feature($"AchievementFieldId_{tag}"))
                .Get();

            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(FeatureService)))
            {
                return (await builder.SendAsync<AchievementFieldId>(httpClient, CancellationToken.None).ConfigureAwait(false)).Body;
            }
        }).ConfigureAwait(false);
    }
}
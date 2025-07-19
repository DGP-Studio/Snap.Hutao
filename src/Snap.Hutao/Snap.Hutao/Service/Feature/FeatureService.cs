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

    public ValueTask<IslandFeature?> GetGameIslandFeatureAsync(string tag)
    {
        return GetTaggedFeatureAsync<IslandFeature>("UnlockerIsland_Compact2", tag, TimeSpan.FromMinutes(5));
    }

    public ValueTask<AchievementFieldId?> GetAchievementFieldIdFeatureAsync(string tag)
    {
        return GetTaggedFeatureAsync<AchievementFieldId>("AchievementFieldId", tag, TimeSpan.FromHours(6));
    }

    public async ValueTask<TFeature?> GetTaggedFeatureAsync<TFeature>(string featureName, string tag, TimeSpan expiration)
        where TFeature : class
    {
        return await memoryCache.GetOrCreateAsync($"{nameof(FeatureService)}.{typeof(TFeature).Name}.{tag}", async entry =>
        {
            entry.SetSlidingExpiration(expiration);
            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory
                .Create()
                .SetRequestUri(hutaoEndpointsFactory.Create().Feature($"{featureName}_{tag}"))
                .Get();

            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(FeatureService)))
            {
                return (await builder.SendAsync<TFeature>(httpClient, CancellationToken.None).ConfigureAwait(false)).Body;
            }
        }).ConfigureAwait(false);
    }
}
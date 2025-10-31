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

[Service(ServiceLifetime.Singleton, typeof(IFeatureService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class FeatureService : IFeatureService
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IMemoryCache memoryCache;

    [GeneratedConstructor]
    public partial FeatureService(IServiceProvider serviceProvider);

    public ValueTask<IslandFeature2?> GetIslandFeatureAsync(string tag)
    {
        return GetTaggedFeatureAsync<IslandFeature2>(tag, TimeSpan.FromMinutes(5));
    }

    public ValueTask<AchievementFieldId?> GetAchievementFieldIdAsync(string tag)
    {
        return GetTaggedFeatureAsync<AchievementFieldId>(tag, TimeSpan.FromHours(6));
    }

    private async ValueTask<TFeature?> GetTaggedFeatureAsync<TFeature>(string tag, TimeSpan expiration)
        where TFeature : class
    {
        string featureName = typeof(TFeature).Name;
        return await memoryCache.GetOrCreateAsync($"{nameof(FeatureService)}.{featureName}.{tag}", async entry =>
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
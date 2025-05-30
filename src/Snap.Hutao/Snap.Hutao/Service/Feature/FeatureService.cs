// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Extensions.Caching.Memory;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game.Island;
using Snap.Hutao.Service.Yae.Achievement;
using Snap.Hutao.Web.Endpoint.Hutao;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Service.Feature;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IFeatureService))]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class FeatureService : IFeatureService
{
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMemoryCache memoryCache;

    public async ValueTask<IslandFeature?> GetGameIslandFeatureAsync(string tag)
    {
        return await memoryCache.GetOrCreateAsync($"{nameof(FeatureService)}.GameIslandFeature.{tag}", async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(5));
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(FeatureService)))
                {
                    string url = hutaoEndpointsFactory.Create().Feature($"UnlockerIsland_Compact2_{tag}");
                    return await httpClient.GetFromJsonAsync<IslandFeature>(url).ConfigureAwait(false);
                }
            }
        }).ConfigureAwait(false);
    }

    public async ValueTask<AchievementFieldId?> GetAchievementFieldIdFeatureAsync(string tag)
    {
        return await memoryCache.GetOrCreateAsync($"{nameof(FeatureService)}.AchievementFieldIdFeature.{tag}", async entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromHours(6));
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(FeatureService)))
                {
                    string url = hutaoEndpointsFactory.Create().Feature($"AchievementFieldId_{tag}");
                    return await httpClient.GetFromJsonAsync<AchievementFieldId>(url).ConfigureAwait(false);
                }
            }
        }).ConfigureAwait(false);
    }
}
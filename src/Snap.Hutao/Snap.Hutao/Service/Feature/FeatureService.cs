// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game.Unlocker.Island;
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

    public async ValueTask<IslandFeature?> GetIslandFeatureAsync()
    {
        using (IServiceScope scope = serviceScopeFactory.CreateScope())
        {
            IHttpClientFactory httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
            using (HttpClient httpClient = httpClientFactory.CreateClient(nameof(FeatureService)))
            {
                try
                {
                    string url = hutaoEndpointsFactory.Create().Feature("UnlockerIsland");
                    return await httpClient.GetFromJsonAsync<IslandFeature>(url).ConfigureAwait(false);
                }
                catch
                {
                    return default;
                }
            }
        }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Wallpaper;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoWallpaperClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly ILogger<HutaoWallpaperClient> logger;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HutaoWallpaperClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public ValueTask<Response<Wallpaper>> GetBingWallpaperAsync(CancellationToken token = default)
    {
        return GetWallpaperAsync(hutaoEndpointsFactory.Create().WallpaperBing(), token);
    }

    public ValueTask<Response<Wallpaper>> GetLauncherWallpaperAsync(CancellationToken token = default)
    {
        return GetWallpaperAsync(hutaoEndpointsFactory.Create().WallpaperGenshinLauncher(), token);
    }

    public ValueTask<Response<Wallpaper>> GetTodayWallpaperAsync(CancellationToken token = default)
    {
        return GetWallpaperAsync(hutaoEndpointsFactory.Create().WallpaperToday(), token);
    }

    private async ValueTask<Response<Wallpaper>> GetWallpaperAsync(string url, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        Response<Wallpaper>? resp = await builder.SendAsync<Response<Wallpaper>>(httpClient, token).ConfigureAwait(false);
        return Web.Response.Response.DefaultIfNull(resp);
    }
}
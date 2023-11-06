// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 游戏资源客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class ResourceClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly HttpClient httpClient;
    private readonly ILogger<ResourceClient> logger;

    /// <summary>
    /// 异步获取游戏资源
    /// </summary>
    /// <param name="scheme">方案</param>
    /// <param name="token">取消令牌</param>
    /// <returns>游戏资源</returns>
    public async ValueTask<Response<GameResource>> GetResourceAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        string url = scheme.IsOversea
            ? ApiOsEndpoints.SdkOsStaticLauncherResource(scheme)
            : ApiEndpoints.SdkStaticLauncherResource(scheme);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        Response<GameResource>? resp = await builder
            .TryCatchSendAsync<Response<GameResource>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // 补全缺失的信息
        if (resp is { Data.Game.Latest: LatestPackage latest })
        {
            latest.Path = latest.Segments[0].Path[..^4]; // .00X
            latest.Name = Path.GetFileName(latest.Path);
        }

        return Response.Response.DefaultIfNull(resp);
    }
}
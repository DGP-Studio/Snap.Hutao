// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Game;
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
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
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

        Response<GameResource>? response = await httpClient
            .TryCatchGetFromJsonAsync<Response<GameResource>>(url, options, logger, token)
            .ConfigureAwait(false);

        // 补全缺失的信息
        if (response is { Data.Game.Latest: LatestPackage latest })
        {
            latest.Path = latest.Segments[0].Path[..^4];
            latest.Name = Path.GetFileName(latest.Path);
        }

        return Response.Response.DefaultIfNull(response);
    }
}
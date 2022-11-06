// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.SdkStatic.Hk4e.Launcher;

/// <summary>
/// 游戏资源客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class ResourceClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<ResourceClient> logger;

    /// <summary>
    /// 构造一个新的资源客户端
    /// </summary>
    /// <param name="httpClient">Http客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public ResourceClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<ResourceClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取游戏资源
    /// </summary>
    /// <param name="scheme">方案</param>
    /// <param name="token">取消令牌</param>
    /// <returns>游戏资源</returns>
    public async Task<GameResource?> GetResourceAsync(LaunchScheme scheme, CancellationToken token = default)
    {
        Response<GameResource>? response = await httpClient
            .TryCatchGetFromJsonAsync<Response<GameResource>>(ApiEndpoints.SdkStaticLauncherResource(scheme.Channel, scheme.SubChannel), options, logger, token)
            .ConfigureAwait(false);

        return response?.Data;
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 更新客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class PatchClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaClient> logger;

    /// <summary>
    /// 构造一个新的更新客户端
    /// </summary>
    /// <param name="httpClient">http 客户端</param>
    /// <param name="options">json选项</param>
    /// <param name="logger">日志器</param>
    public PatchClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<HomaClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取更新信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>更新信息</returns>
    public Task<Patch.PatchInformation?> GetPatchInformationAsync(CancellationToken token = default)
    {
        return httpClient.TryCatchGetFromJsonAsync<Patch.PatchInformation>(HutaoEndpoints.PatcherHutaoStable, options, logger, token);
    }
}
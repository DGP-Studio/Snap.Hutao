// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using System.Net.Http;

namespace Snap.Hutao.Web.Enka;

/// <summary>
/// Enka API 客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class EnkaClient
{
    private const string EnkaAPI = "https://enka.network/u/{0}/__data.json";
    private const string EnkaAPIHutaoForward = "https://enka-api.hut.ao/{0}";

    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<EnkaClient> logger;

    /// <summary>
    /// 构造一个新的 Enka API 客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">序列化选项</param>
    /// <param name="logger">日志器</param>
    public EnkaClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<EnkaClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取转发的 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public Task<EnkaResponse?> GetForwardDataAsync(PlayerUid playerUid, CancellationToken token = default)
    {
        return httpClient.TryCatchGetFromJsonAsync<EnkaResponse>(string.Format(EnkaAPIHutaoForward, playerUid.Value), options, logger, token);
    }

    /// <summary>
    /// 异步获取 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public Task<EnkaResponse?> GetDataAsync(PlayerUid playerUid, CancellationToken token = default)
    {
        return httpClient.TryCatchGetFromJsonAsync<EnkaResponse>(string.Format(EnkaAPI, playerUid.Value), options, logger, token);
    }
}
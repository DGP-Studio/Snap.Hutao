// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Enka.Model;
using Snap.Hutao.Web.Hoyolab;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Enka;

/// <summary>
/// Enka API 客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class EnkaClient
{
    private const string EnkaAPIHutaoForward = "https://enka-api.hut.ao/{0}";

    private readonly HttpClient httpClient;

    /// <summary>
    /// 构造一个新的 Enka API 客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    public EnkaClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    /// <summary>
    /// 异步获取 Enka API 响应
    /// </summary>
    /// <param name="playerUid">玩家Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Enka API 响应</returns>
    public Task<EnkaResponse?> GetDataAsync(PlayerUid playerUid, CancellationToken token)
    {
        return httpClient.GetFromJsonAsync<EnkaResponse>(string.Format(EnkaAPIHutaoForward, playerUid.Value), token);
    }
}

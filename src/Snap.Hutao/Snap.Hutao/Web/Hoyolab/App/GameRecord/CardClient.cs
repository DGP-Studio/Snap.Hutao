// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.App.GameRecord;

/// <summary>
/// 桌面卡片客户端
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class CardClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<CardClient> logger;

    /// <summary>
    /// 构造一个新的桌面卡片客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public CardClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<CardClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取桌面小组件数据
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>桌面小组件数据</returns>
    [ApiInformation<WidgetData>(Cookie = CookieType.Stoken, Salt = SaltType.X6)]
    public async Task<WidgetData?> GetWidgetDataAsync(User user, CancellationToken token)
    {
        Response<DataWrapper<WidgetData>>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UsingDynamicSecret2(SaltType.X6, options, ApiEndpoints.AppWidgetData)
            .TryCatchGetFromJsonAsync<Response<DataWrapper<WidgetData>>>(logger, token)
            .ConfigureAwait(false);

        return resp?.Data?.Data;
    }
}
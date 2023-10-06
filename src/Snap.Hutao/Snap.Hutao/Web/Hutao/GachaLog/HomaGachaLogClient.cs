// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 胡桃祈愿记录API客户端
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HomaGachaLogClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HomaGachaLogClient> logger;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取祈愿统计信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>祈愿统计信息</returns>
    public async ValueTask<Response<GachaEventStatistics>> GetGachaEventStatisticsAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogStatisticsCurrentEvents)
            .Get();

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<GachaEventStatistics>? resp = await builder
            .TryCatchSendAsync<Response<GachaEventStatistics>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取祈愿分布
    /// </summary>
    /// <param name="distributionType">分布类型</param>
    /// <param name="token">取消令牌</param>
    /// <returns>祈愿分布</returns>
    public async ValueTask<Response<GachaDistribution>> GetGachaDistributionAsync(GachaDistributionType distributionType, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogStatisticsDistribution(distributionType))
            .Get();

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<GachaDistribution>? resp = await builder
            .TryCatchSendAsync<Response<GachaDistribution>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 Uid 列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Uid 列表</returns>
    public async ValueTask<Response<List<GachaEntry>>> GetGachaEntriesAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogEntries)
            .Get();

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<List<GachaEntry>>? resp = await builder
            .TryCatchSendAsync<Response<List<GachaEntry>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取末尾 Id
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>末尾Id</returns>
    public async ValueTask<Response<EndIds>> GetEndIdsAsync(string uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogEndIds(uid))
            .Get();

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<EndIds>? resp = await builder
            .TryCatchSendAsync<Response<EndIds>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取云端祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="endIds">末尾 Id</param>
    /// <param name="token">取消令牌</param>
    /// <returns>云端祈愿记录</returns>
    public async ValueTask<Response<List<GachaItem>>> RetrieveGachaItemsAsync(string uid, EndIds endIds, CancellationToken token = default)
    {
        UidAndEndIds uidAndEndIds = new(uid, endIds);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogRetrieve)
            .PostJson(uidAndEndIds);

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<List<GachaItem>>? resp = await builder
            .TryCatchSendAsync<Response<List<GachaItem>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步上传祈愿记录物品
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="gachaItems">祈愿记录</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    public async ValueTask<Response.Response> UploadGachaItemsAsync(string uid, List<GachaItem> gachaItems, CancellationToken token = default)
    {
        UidAndItems uidAndItems = new(uid, gachaItems);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogUpload)
            .PostJson(uidAndItems);

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response.Response? resp = await builder
            .TryCatchSendAsync<Response.Response>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步删除祈愿记录
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    public async ValueTask<Response.Response> DeleteGachaItemsAsync(string uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.GachaLogDelete(uid))
            .Get();

        await builder.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response.Response? resp = await builder
            .TryCatchSendAsync<Response.Response>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private sealed class UidAndEndIds
    {
        public UidAndEndIds(string uid, EndIds endIds)
        {
            Uid = uid;
            EndIds = endIds;
        }

        public string Uid { get; }

        public EndIds EndIds { get; }
    }

    private sealed class UidAndItems
    {
        public UidAndItems(string uid, List<GachaItem> gachaItems)
        {
            Uid = uid;
            Items = gachaItems;
        }

        public string Uid { get; set; } = default!;

        public List<GachaItem> Items { get; set; } = default!;
    }
}
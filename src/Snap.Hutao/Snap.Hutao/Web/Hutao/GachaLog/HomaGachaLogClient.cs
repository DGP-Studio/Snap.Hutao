// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
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
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaGachaLogClient> logger;
    private readonly HutaoUserOptions hutaoUserOptions;

    /// <summary>
    /// 异步获取祈愿统计信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>祈愿统计信息</returns>
    public async ValueTask<Response<GachaEventStatistics>> GetGachaEventStatisticsAsync(CancellationToken token = default)
    {
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<GachaEventStatistics>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<GachaEventStatistics>>(HutaoEndpoints.GachaLogStatisticsCurrentEvents, options, logger, token)
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
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<GachaDistribution>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<GachaDistribution>>(HutaoEndpoints.GachaLogStatisticsDistribution(distributionType), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 Uid 列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Uid 列表</returns>
    [Obsolete("Use GetGachaEntriesAsync instead")]
    public async ValueTask<Response<List<string>>> GetUidsAsync(CancellationToken token = default)
    {
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<List<string>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<string>>>(HutaoEndpoints.GachaLogUids, options, logger, token)
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
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<List<GachaEntry>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<GachaEntry>>>(HutaoEndpoints.GachaLogEntries, options, logger, token)
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
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response<EndIds>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<EndIds>>(HutaoEndpoints.GachaLogEndIds(uid), options, logger, token)
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
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        UidAndEndIds uidAndEndIds = new(uid, endIds);

        Response<List<GachaItem>>? resp = await httpClient
            .TryCatchPostAsJsonAsync<UidAndEndIds, Response<List<GachaItem>>>(HutaoEndpoints.GachaLogRetrieve, uidAndEndIds, options, logger, token)
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
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        UidAndItems uidAndItems = new(uid, gachaItems);

        Response.Response? resp = await httpClient
            .TryCatchPostAsJsonAsync<UidAndItems, Response<List<GachaItem>>>(HutaoEndpoints.GachaLogUpload, uidAndItems, options, logger, token)
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
        await httpClient.TrySetTokenAsync(hutaoUserOptions).ConfigureAwait(false);

        Response.Response? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<GachaItem>>>(HutaoEndpoints.GachaLogDelete(uid), options, logger, token)
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
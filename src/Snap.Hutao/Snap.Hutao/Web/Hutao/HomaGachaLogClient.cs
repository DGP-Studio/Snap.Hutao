// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃祈愿记录API客户端
/// </summary>
[HttpClient(HttpClientConfiguration.Default)]
internal sealed class HomaGachaLogClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<HomaGachaLogClient> logger;

    /// <summary>
    /// 构造一个新的胡桃祈愿记录API客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="serviceProvider">服务提供器</param>
    public HomaGachaLogClient(HttpClient httpClient, IServiceProvider serviceProvider)
    {
        options = serviceProvider.GetRequiredService<JsonSerializerOptions>();
        logger = serviceProvider.GetRequiredService<ILogger<HomaGachaLogClient>>();

        this.httpClient = httpClient;

        HutaoUserOptions hutaoUserOptions = serviceProvider.GetRequiredService<HutaoUserOptions>();
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", hutaoUserOptions.Token);
    }

    /// <summary>
    /// 异步获取 Uid 列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>Uid 列表</returns>
    public async Task<Response<List<string>>> GetUidsAsync(CancellationToken token = default)
    {
        Response<List<string>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<string>>>(HutaoEndpoints.GachaLogUids, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取末尾 Id
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>末尾Id</returns>
    public async Task<Response<EndIds>> GetEndIdsAsync(PlayerUid uid, CancellationToken token = default)
    {
        Response<EndIds>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<EndIds>>(HutaoEndpoints.GachaLogEndIds(uid.Value), options, logger, token)
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
    public async Task<Response<List<GachaItem>>> RetrieveGachaItemsAsync(PlayerUid uid, EndIds endIds, CancellationToken token = default)
    {
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
    public async Task<Response.Response> UploadGachaItemsAsync(PlayerUid uid, List<GachaItem> gachaItems, CancellationToken token = default)
    {
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
    public async Task<Response.Response> DeleteGachaItemsAsync(PlayerUid uid, CancellationToken token = default)
    {
        Response.Response? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<GachaItem>>>(HutaoEndpoints.GachaLogDelete(uid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private sealed class UidAndEndIds
    {
        public UidAndEndIds(PlayerUid uid, EndIds endIds)
        {
            Uid = uid.Value;
            EndIds = endIds;
        }

        public string Uid { get; }

        public EndIds EndIds { get; }
    }

    private sealed class UidAndItems
    {
        public UidAndItems(PlayerUid uid, List<GachaItem> gachaItems)
        {
            Uid = uid.Value;
            Items = gachaItems;
        }

        public string Uid { get; set; } = default!;

        public List<GachaItem> Items { get; set; } = default!;
    }
}
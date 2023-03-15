// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 养成计算器客户端
/// </summary>
[HighQuality]
[HttpClient(HttpClientConfigration.Default)]
internal sealed class CalculateClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<CalculateClient> logger;

    /// <summary>
    /// 构造一个新的养成计算器客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public CalculateClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<CalculateClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步计算结果
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="delta">差异</param>
    /// <param name="token">取消令牌</param>
    /// <returns>消耗结果</returns>
    [ApiInformation(Cookie = CookieType.Cookie)]
    public async Task<Response<Consumption>> ComputeAsync(Model.Entity.User user, AvatarPromotionDelta delta, CancellationToken token = default)
    {
        Response<Consumption>? resp;
        // TODO: need a flag to identify hoyoverse account
        if (user.Stoken == null)
        {
            resp = await httpClient
            .SetUser(user, CookieType.Cookie)
            .SetReferer(ApiOsEndpoints.ActHoyolabReferer)
            .TryCatchPostAsJsonAsync<AvatarPromotionDelta, Response<Consumption>>(ApiOsEndpoints.CalculateOsCompute, delta, options, logger, token)
            .ConfigureAwait(false);
        }
        else
        {
            resp = await httpClient
            .SetUser(user, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .TryCatchPostAsJsonAsync<AvatarPromotionDelta, Response<Consumption>>(ApiEndpoints.CalculateCompute, delta, options, logger, token)
            .ConfigureAwait(false);
        }

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<Avatar>> GetAvatarsAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        int currentPage = 1;
        SyncAvatarFilter filter = new() { Uid = userAndUid.Uid.Value, Region = userAndUid.Uid.Region };

        List<Avatar> avatars = new();
        Response<ListWrapper<Avatar>>? resp;

        // 根据 uid 所属服务器选择 referer 与 api
        string referer = ApiOsEndpoints.ActHoyolabReferer;
        string endpoint = ApiOsEndpoints.CalculateOsSyncAvatarList;

        if (userAndUid.Uid.Region == "cn_gf01" || userAndUid.Uid.Region == "cn_qd01")
        {
            referer = ApiEndpoints.WebStaticMihoyoReferer;
            endpoint = ApiEndpoints.CalculateSyncAvatarList;
            httpClient.SetUser(userAndUid.User, CookieType.CookieToken);
        }
        else
        {
            httpClient.SetUser(userAndUid.User, CookieType.Cookie);
        }

        do
        {
            filter.Page = currentPage++;
            resp = await httpClient
                .SetReferer(referer)
                .TryCatchPostAsJsonAsync<SyncAvatarFilter, Response<ListWrapper<Avatar>>>(endpoint, filter, options, logger, token)
                .ConfigureAwait(false);

            if (resp != null && resp.IsOk())
            {
                avatars.AddRange(resp.Data.List);
            }
            else
            {
                // Hot path to exit loop
                break;
            }

            await Task.Delay(Random.Shared.Next(0, 1000), token).ConfigureAwait(false);
        }
        while (resp?.Data?.List?.Count == 20);

        return avatars;
    }

    /// <summary>
    /// 异步获取角色详情
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="avatar">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色详情</returns>
    public async Task<Response<AvatarDetail>> GetAvatarDetailAsync(UserAndUid userAndUid, Avatar avatar, CancellationToken token = default)
    {
        Response<AvatarDetail>? resp;
        if (userAndUid.Uid.Region == "cn_gf01" || userAndUid.Uid.Region == "cn_qd01")
        {
            resp = await httpClient
            .SetUser(userAndUid.User, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<AvatarDetail>>(ApiEndpoints.CalculateSyncAvatarDetail(avatar.Id, userAndUid.Uid.Value), options, logger, token)
            .ConfigureAwait(false);
        }
        else
        {
            resp = await httpClient
            .SetUser(userAndUid.User, CookieType.Cookie)
            .TryCatchGetFromJsonAsync<Response<AvatarDetail>>(ApiOsEndpoints.CalculateOsSyncAvatarDetail(avatar.Id, userAndUid.Uid.Value), options, logger, token)
            .ConfigureAwait(false);
        }
        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取摹本的家具列表
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="shareCode">摹本码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>家具列表</returns>
    public async Task<Response<FurnitureListWrapper>> FurnitureBlueprintAsync(Model.Entity.User user, string shareCode, CancellationToken token)
    {
        Response<FurnitureListWrapper>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<FurnitureListWrapper>>(ApiEndpoints.CalculateFurnitureBlueprint(shareCode), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 家具数量计算
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="items">物品</param>
    /// <param name="token">取消令牌</param>
    /// <returns>消耗</returns>
    public async Task<Response<ListWrapper<Item>>> FurnitureComputeAsync(Model.Entity.User user, List<Item> items, CancellationToken token)
    {
        ListWrapper<IdCount> data = new() { List = items.Select(i => new IdCount { Id = i.Id, Count = i.Num }).ToList() };

        Response<ListWrapper<Item>>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchPostAsJsonAsync<ListWrapper<IdCount>, Response<ListWrapper<Item>>>(ApiEndpoints.CalculateFurnitureCompute, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private class SyncAvatarFilter
    {
        [JsonPropertyName("element_attr_ids")]
        public List<int>? ElementAttrIds { get; set; } = new();

        [JsonPropertyName("weapon_cat_ids")]
        public List<int>? WeaponCatIds { get; set; } = new();

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; } = 20;

        [JsonPropertyName("uid")]
        public string Uid { get; set; } = default!;

        [JsonPropertyName("region")]
        public string Region { get; set; } = default!;
    }

    private class IdCount
    {
        [JsonPropertyName("cnt")]
        public int Count { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
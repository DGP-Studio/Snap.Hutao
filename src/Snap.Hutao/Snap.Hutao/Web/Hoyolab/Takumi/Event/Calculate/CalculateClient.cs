// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

/// <summary>
/// 养成计算器客户端
/// </summary>
[HttpClient(HttpClientConfigration.Default)]
internal class CalculateClient
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
    public async Task<Consumption?> ComputeAsync(User user, AvatarPromotionDelta delta, CancellationToken token = default)
    {
        Response<Consumption>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchPostAsJsonAsync<AvatarPromotionDelta, Response<Consumption>>(ApiEndpoints.CalculateCompute, delta, options, logger, token)
            .ConfigureAwait(false);
        return resp?.Data;
    }

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<Avatar>> GetAvatarsAsync(User user, PlayerUid uid, CancellationToken token = default)
    {
        int currentPage = 1;
        SyncAvatarFilter filter = new() { Uid = uid.Value, Region = uid.Region };

        List<Avatar> avatars = new();
        Response<ListWrapper<Avatar>>? resp;
        httpClient.SetUser(user, CookieType.CookieToken);

        do
        {
            filter.Page = currentPage++;
            resp = await httpClient
                .TryCatchPostAsJsonAsync<SyncAvatarFilter, Response<ListWrapper<Avatar>>>(ApiEndpoints.CalculateSyncAvatarList, filter, options, logger, token)
                .ConfigureAwait(false);

            if (resp?.Data?.List is not null)
            {
                avatars.AddRange(resp.Data.List);
            }

            await Task.Delay(Random.Shared.Next(0, 1000), token).ConfigureAwait(false);
        }
        while (resp?.Data?.List?.Count == 20);

        return avatars;
    }

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<Avatar>> GetAvatarsAsync(User user, CancellationToken token = default)
    {
        int currentPage = 1;
        AvatarFilter filter = new();

        List<Avatar> avatars = new();
        Response<ListWrapper<Avatar>>? resp;
        httpClient.SetUser(user, CookieType.CookieToken);

        do
        {
            filter.Page = currentPage++;
            resp = await httpClient
                .TryCatchPostAsJsonAsync<AvatarFilter, Response<ListWrapper<Avatar>>>(ApiEndpoints.CalculateAvatarList, filter, options, logger, token)
                .ConfigureAwait(false);

            if (resp?.Data?.List is not null)
            {
                avatars.AddRange(resp.Data.List);
            }

            await Task.Delay(Random.Shared.Next(0, 1000), token).ConfigureAwait(false);
        }
        while (resp?.Data?.List?.Count == 20);

        return avatars;
    }

    /// <summary>
    /// 异步获取角色详情
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">uid</param>
    /// <param name="avatar">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色详情</returns>
    public async Task<AvatarDetail?> GetAvatarDetailAsync(User user, PlayerUid uid, Avatar avatar, CancellationToken token = default)
    {
        Response<AvatarDetail>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<AvatarDetail>>(ApiEndpoints.CalculateSyncAvatarDetail(avatar.Id, uid), options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取角色技能列表
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="avatar">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色技能列表</returns>
    public async Task<List<Skill>> GetAvatarSkillsAsync(User user, Avatar avatar, CancellationToken token)
    {
        Response<ListWrapper<Skill>>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<ListWrapper<Skill>>>(ApiEndpoints.CalculateAvatarSkillList(avatar), options, logger, token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data?.List);
    }

    /// <summary>
    /// 异步获取角色列表
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<Weapon>> GetWeaponsAsync(User user, CancellationToken token)
    {
        int currentPage = 1;
        WeaponFilter filter = new();

        List<Weapon> weapons = new();
        Response<ListWrapper<Weapon>>? resp;
        httpClient.SetUser(user, CookieType.CookieToken);

        do
        {
            filter.Page = currentPage++;
            resp = await httpClient
                .TryCatchPostAsJsonAsync<WeaponFilter, Response<ListWrapper<Weapon>>>(ApiEndpoints.CalculateWeaponList, filter, options, logger, token)
                .ConfigureAwait(false);

            if (resp?.Data?.List is not null)
            {
                weapons.AddRange(resp.Data.List);
            }

            await Task.Delay(Random.Shared.Next(0, 1000), token).ConfigureAwait(false);
        }
        while (resp?.Data?.List?.Count == 20);

        return weapons;
    }

    /// <summary>
    /// 异步获取摹本的家具列表
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="shareCode">摹本码</param>
    /// <param name="token">取消令牌</param>
    /// <returns>家具列表</returns>
    public async Task<FurnitureListWrapper?> FurnitureBlueprintAsync(User user, string shareCode, CancellationToken token)
    {
        Response<FurnitureListWrapper>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchGetFromJsonAsync<Response<FurnitureListWrapper>>(ApiEndpoints.CalculateFurnitureBlueprint(shareCode), options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 家具数量计算
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="items">物品</param>
    /// <param name="token">取消令牌</param>
    /// <returns>消耗</returns>
    public async Task<List<Item>> FurnitureComputeAsync(User user, List<Item> items, CancellationToken token)
    {
        ListWrapper<IdCount> data = new() { List = items.Select(i => new IdCount { Id = i.Id, Count = i.Num }).ToList() };

        Response<ListWrapper<Item>>? resp = await httpClient
            .SetUser(user, CookieType.CookieToken)
            .TryCatchPostAsJsonAsync<ListWrapper<IdCount>, Response<ListWrapper<Item>>>(ApiEndpoints.CalculateFurnitureCompute, data, options, logger, token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data?.List);
    }

    private class AvatarFilter
    {
        [JsonPropertyName("element_attr_ids")]
        public List<int>? ElementAttrIds { get; set; } = new();

        [JsonPropertyName("weapon_cat_ids")]
        public List<int>? WeaponCatIds { get; set; } = new();

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; } = 20;

        [JsonPropertyName("is_all")]
        public bool IsAll { get; set; } = true;
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

    private class WeaponFilter
    {
        [JsonPropertyName("weapon_levels")]
        public List<int> WeaponLevels { get; set; } = new();

        [JsonPropertyName("weapon_cat_ids")]
        public List<int> WeaponCatIds { get; set; } = new();

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; } = 20;
    }

    private class IdCount
    {
        [JsonPropertyName("cnt")]
        public int Count { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }
    }
}
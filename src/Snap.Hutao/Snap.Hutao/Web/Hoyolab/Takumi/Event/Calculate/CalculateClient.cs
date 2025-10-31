// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class CalculateClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IApiEndpointsFactory apiEndpointsFactory;
    private readonly IServiceProvider serviceProvider;

    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial CalculateClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<BatchConsumption>> BatchComputeAsync(UserAndUid userAndUid, AvatarPromotionDelta delta, bool syncInventory = false, CancellationToken token = default)
    {
        return await BatchComputeAsync(userAndUid, [delta], syncInventory, token).ConfigureAwait(false);
    }

    public async ValueTask<Response<BatchConsumption>> BatchComputeAsync(UserAndUid userAndUid, ImmutableArray<AvatarPromotionDelta> deltas, bool syncInventory = false, CancellationToken token = default)
    {
        BatchConsumptionData data = new()
        {
            Items = deltas,
            Region = syncInventory ? userAndUid.Uid.Region : default!,
            Uid = syncInventory ? userAndUid.Uid.ToString() : default!,
        };

        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(userAndUid.IsOversea);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CalculateBatchCompute())
            .SetUserCookieAndFpHeader(userAndUid.User, CookieType.Cookie)
            .SetReferer(userAndUid.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
            .PostJson(data);

        Response<BatchConsumption>? resp = await builder
            .SendAsync<Response<BatchConsumption>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<ImmutableArray<Avatar>> GetAvatarsAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        int currentPage = 1;
        SyncAvatarFilter filter = new() { Uid = userAndUid.Uid.Value, Region = userAndUid.Uid.Region };

        ImmutableArray<Avatar>.Builder avatars = ImmutableArray.CreateBuilder<Avatar>();
        Response<ListWrapper<Avatar>>? resp;

        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(userAndUid.IsOversea);

        do
        {
            filter.Page = currentPage++;

            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetRequestUri(apiEndpoints.CalculateSyncAvatarList())
                .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
                .SetReferer(userAndUid.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
                .PostJson(filter);

            resp = await builder
                .SendAsync<Response<ListWrapper<Avatar>>>(httpClient, token)
                .ConfigureAwait(false);

            Response.Response.DefaultIfNull(ref resp);

            if (ResponseValidator.TryValidate(resp, serviceProvider, out ListWrapper<Avatar>? listWrapper))
            {
                avatars.AddRange(listWrapper.List);
            }
            else
            {
                // Hot path to exit loop
                break;
            }

            await Task.Delay(Random.Shared.Next(0, 1000), token).ConfigureAwait(false);
        }
        while (resp.Data is { List.Length: 20 });

        return avatars.ToImmutable();
    }

    public async ValueTask<Response<AvatarDetail>> GetAvatarDetailAsync(UserAndUid userAndUid, Avatar avatar, CancellationToken token = default)
    {
        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(userAndUid.IsOversea);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CalculateSyncAvatarDetail(avatar.Id, userAndUid.Uid.Value))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetReferer(userAndUid.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
            .Get();

        Response<AvatarDetail>? resp = await builder
            .SendAsync<Response<AvatarDetail>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<FurnitureListWrapper>> FurnitureBlueprintAsync(Model.Entity.User user, string shareCode, CancellationToken token)
    {
        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(user.IsOversea);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CalculateFurnitureBlueprint(shareCode))
            .SetUserCookieAndFpHeader(user, CookieType.CookieToken)
            .SetReferer(user.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
            .Get();

        Response<FurnitureListWrapper>? resp = await builder
            .SendAsync<Response<FurnitureListWrapper>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<Item>>> FurnitureComputeAsync(Model.Entity.User user, ImmutableArray<Item> items, CancellationToken token)
    {
        ListWrapper<IdCount> data = new() { List = items.SelectAsArray(static i => new IdCount { Id = i.Id, Count = i.Num }) };

        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(user.IsOversea);
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CalculateFurnitureCompute())
            .SetUserCookieAndFpHeader(user, CookieType.CookieToken)
            .SetReferer(user.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
            .PostJson(data);

        Response<ListWrapper<Item>>? resp = await builder
            .SendAsync<Response<ListWrapper<Item>>>(httpClient, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<ImmutableArray<Avatar>> GetAllAvatarsAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        SyncAvatarFilter filter = new() { Page = 1, Size = 1000, IsAll = true };

        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(userAndUid.IsOversea);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CalculateAvatarList())
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetReferer(userAndUid.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
            .PostJson(filter);

        Response<ListWrapper<Avatar>>? resp = await builder
            .SendAsync<Response<ListWrapper<Avatar>>>(httpClient, token)
            .ConfigureAwait(false);

        Response.Response.DefaultIfNull(ref resp);

        if (ResponseValidator.TryValidate(resp, serviceProvider, out ListWrapper<Avatar>? listWrapper))
        {
            return listWrapper.List;
        }

        return [];
    }

    public async ValueTask<ImmutableArray<Weapon>> GetAllWeaponsAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        SyncWeaponFilter filter = new() { WeaponLevels = [4, 5] };

        IApiEndpoints apiEndpoints = apiEndpointsFactory.Create(userAndUid.IsOversea);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.CalculateWeaponList())
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetReferer(userAndUid.IsOversea ? apiEndpoints.ActHoyolabReferer() : apiEndpoints.WebStaticReferer())
            .PostJson(filter);

        Response<ListWrapper<Weapon>>? resp = await builder
            .SendAsync<Response<ListWrapper<Weapon>>>(httpClient, token)
            .ConfigureAwait(false);

        Response.Response.DefaultIfNull(ref resp);

        if (ResponseValidator.TryValidate(resp, serviceProvider, out ListWrapper<Weapon>? listWrapper))
        {
            return listWrapper.List;
        }

        return [];
    }

    private class SyncAvatarFilter
    {
        [JsonPropertyName("element_attr_ids")]
        public List<int>? ElementAttrIds { get; set; } = [];

        [JsonPropertyName("weapon_cat_ids")]
        public List<int>? WeaponCatIds { get; set; } = [];

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; } = 20;

        [JsonPropertyName("is_all")]
        public bool? IsAll { get; set; }

        [JsonPropertyName("uid")]
        public string? Uid { get; set; }

        [JsonPropertyName("region")]
        public Region? Region { get; set; }
    }

    private class SyncWeaponFilter
    {
        [JsonPropertyName("weapon_cat_ids")]
        public List<int>? WeaponCatIds { get; set; } = [];

        [JsonPropertyName("weapon_levels")]
        public List<int>? WeaponLevels { get; set; } = [];

        [JsonPropertyName("page")]
        public int Page { get; set; } = 1;

        [JsonPropertyName("size")]
        public int Size { get; set; } = 1000;
    }

    private class IdCount
    {
        [JsonPropertyName("cnt")]
        public uint Count { get; set; }

        [JsonPropertyName("id")]
        public uint Id { get; set; }
    }

    private class BatchConsumptionData
    {
        [JsonPropertyName("items")]
        public required ImmutableArray<AvatarPromotionDelta> Items { get; init; }

        [JsonPropertyName("region")]
        public required Region Region { get; init; }

        [JsonPropertyName("uid")]
        public required string Uid { get; init; }
    }
}
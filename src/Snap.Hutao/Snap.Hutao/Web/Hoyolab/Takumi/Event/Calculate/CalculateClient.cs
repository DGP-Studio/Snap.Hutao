// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class CalculateClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<CalculateClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<Consumption>> ComputeAsync(Model.Entity.User user, AvatarPromotionDelta delta, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(user.IsOversea ? ApiOsEndpoints.CalculateCompute : ApiEndpoints.CalculateCompute)
            .SetUserCookieAndFpHeader(user, CookieType.Cookie)
            .SetReferer(user.IsOversea ? ApiOsEndpoints.ActHoyolabReferer : ApiEndpoints.WebStaticMihoyoReferer)
            .PostJson(delta);

        Response<Consumption>? resp = await builder
            .TryCatchSendAsync<Response<Consumption>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<List<Avatar>> GetAvatarsAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        int currentPage = 1;
        SyncAvatarFilter filter = new() { Uid = userAndUid.Uid.Value, Region = userAndUid.Uid.Region };

        List<Avatar> avatars = [];
        Response<ListWrapper<Avatar>>? resp;

        do
        {
            filter.Page = currentPage++;

            HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
                .SetRequestUri(userAndUid.User.IsOversea ? ApiOsEndpoints.CalculateSyncAvatarList : ApiEndpoints.CalculateSyncAvatarList)
                .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
                .SetReferer(userAndUid.User.IsOversea ? ApiOsEndpoints.ActHoyolabReferer : ApiEndpoints.WebStaticMihoyoReferer)
                .PostJson(filter);

            resp = await builder
                .TryCatchSendAsync<Response<ListWrapper<Avatar>>>(httpClient, logger, token)
                .ConfigureAwait(false);

            if (resp is not null && resp.IsOk())
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
        while (resp.Data is { List.Count: 20 });

        return avatars;
    }

    public async ValueTask<Response<AvatarDetail>> GetAvatarDetailAsync(UserAndUid userAndUid, Avatar avatar, CancellationToken token = default)
    {
        string url = userAndUid.User.IsOversea
            ? ApiOsEndpoints.CalculateSyncAvatarDetail(avatar.Id, userAndUid.Uid.Value)
            : ApiEndpoints.CalculateSyncAvatarDetail(avatar.Id, userAndUid.Uid.Value);

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .SetUserCookieAndFpHeader(userAndUid, CookieType.CookieToken)
            .SetReferer(userAndUid.User.IsOversea ? ApiOsEndpoints.ActHoyolabReferer : ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        Response<AvatarDetail>? resp = await builder
            .TryCatchSendAsync<Response<AvatarDetail>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<FurnitureListWrapper>> FurnitureBlueprintAsync(Model.Entity.User user, string shareCode, CancellationToken token)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CalculateFurnitureBlueprint(shareCode))
            .SetUserCookieAndFpHeader(user, CookieType.CookieToken)
            .SetReferer(user.IsOversea ? ApiOsEndpoints.ActHoyolabReferer : ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        Response<FurnitureListWrapper>? resp = await builder
            .TryCatchSendAsync<Response<FurnitureListWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<Item>>> FurnitureComputeAsync(Model.Entity.User user, List<Item> items, CancellationToken token)
    {
        ListWrapper<IdCount> data = new() { List = items.Select(i => new IdCount { Id = i.Id, Count = i.Num }).ToList() };

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.CalculateFurnitureCompute)
            .SetUserCookieAndFpHeader(user, CookieType.CookieToken)
            .SetReferer(user.IsOversea ? ApiOsEndpoints.ActHoyolabReferer : ApiEndpoints.WebStaticMihoyoReferer)
            .PostJson(data);

        Response<ListWrapper<Item>>? resp = await builder
            .TryCatchSendAsync<Response<ListWrapper<Item>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
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

        [JsonPropertyName("uid")]
        public string Uid { get; set; } = default!;

        [JsonPropertyName("region")]
        public Region Region { get; set; } = default!;
    }

    private class IdCount
    {
        [JsonPropertyName("cnt")]
        public uint Count { get; set; }

        [JsonPropertyName("id")]
        public uint Id { get; set; }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.SpiralAbyss.Post;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoSpiralAbyssClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial HutaoSpiralAbyssClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<HutaoResponse<bool>> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RecordCheck(uid.Value))
            .Get();

        HutaoResponse<bool>? resp = await builder
            .SendAsync<HutaoResponse<bool>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<RankInfo>> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RecordRank(uid.Value))
            .Get();

        HutaoResponse<RankInfo>? resp = await builder
            .SendAsync<HutaoResponse<RankInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<Overview>> GetOverviewAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsOverview(last))
            .Get();

        HutaoResponse<Overview>? resp = await builder
            .SendAsync<HutaoResponse<Overview>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarAttendanceRate(last))
            .Get();

        HutaoResponse<List<AvatarAppearanceRank>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarAppearanceRank>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarUtilizationRate(last))
            .Get();

        HutaoResponse<List<AvatarUsageRank>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarUsageRank>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarCollocation>>> GetAvatarCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarAvatarCollocation(last))
            .Get();

        HutaoResponse<List<AvatarCollocation>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarCollocation>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<WeaponCollocation>>> GetWeaponCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsWeaponWeaponCollocation(last))
            .Get();

        HutaoResponse<List<WeaponCollocation>>? resp = await builder
            .SendAsync<HutaoResponse<List<WeaponCollocation>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarHoldingRate(last))
            .Get();

        HutaoResponse<List<AvatarConstellationInfo>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarConstellationInfo>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<TeamAppearance>>> GetTeamCombinationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsTeamCombination(last))
            .Get();

        HutaoResponse<List<TeamAppearance>>? resp = await builder
            .SendAsync<HutaoResponse<List<TeamAppearance>>>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<SimpleRecord?> GetPlayerRecordAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        IGameRecordClient gameRecordClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.IsOversea);

        // Reduce risk verify chance.
        Response<PlayerInfo> playerInfoResponse = await gameRecordClient
            .GetPlayerInfoAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(playerInfoResponse, serviceProvider))
        {
            return default;
        }

        Response<ListWrapper<Character>> listResponse = await gameRecordClient
            .GetCharacterListAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(listResponse, serviceProvider, out ListWrapper<Character>? charactersWrapper))
        {
            return default;
        }

        Response<ListWrapper<DetailedCharacter>> detailResponse = await gameRecordClient
            .GetCharacterDetailAsync(userAndUid, charactersWrapper.List.SelectAsArray(static c => c.Id), token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(detailResponse, serviceProvider, out ListWrapper<DetailedCharacter>? detailsWrapper))
        {
            return default;
        }

        Response<Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> spiralAbyssResponse = await gameRecordClient
            .GetSpiralAbyssAsync(userAndUid, ScheduleType.Current, token)
            .ConfigureAwait(false);

        if (ResponseValidator.TryValidate(spiralAbyssResponse, serviceProvider, out Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss? spiralAbyss))
        {
            HutaoUserOptions options = serviceProvider.GetRequiredService<HutaoUserOptions>();
            string? userName = await options.GetActualUserNameAsync().ConfigureAwait(false);
            return new(userAndUid.Uid.Value, detailsWrapper.List, spiralAbyss, userName);
        }

        return default;
    }

    public async ValueTask<HutaoResponse> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RecordUpload())
            .PostJson(playerRecord);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}
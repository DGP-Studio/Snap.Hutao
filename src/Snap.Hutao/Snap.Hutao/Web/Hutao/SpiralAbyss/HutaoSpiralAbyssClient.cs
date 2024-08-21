// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.SpiralAbyss.Post;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoSpiralAbyssClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly ILogger<HutaoSpiralAbyssClient> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    public async ValueTask<HutaoResponse<bool>> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RecordCheck(uid.Value))
            .Get();

        HutaoResponse<bool>? resp = await builder
            .SendAsync<HutaoResponse<bool>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<RankInfo>> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RecordRank(uid.Value))
            .Get();

        HutaoResponse<RankInfo>? resp = await builder
            .SendAsync<HutaoResponse<RankInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<Overview>> GetOverviewAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsOverview(last))
            .Get();

        HutaoResponse<Overview>? resp = await builder
            .SendAsync<HutaoResponse<Overview>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarAttendanceRate(last))
            .Get();

        HutaoResponse<List<AvatarAppearanceRank>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarAppearanceRank>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarUtilizationRate(last))
            .Get();

        HutaoResponse<List<AvatarUsageRank>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarUsageRank>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarCollocation>>> GetAvatarCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarAvatarCollocation(last))
            .Get();

        HutaoResponse<List<AvatarCollocation>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarCollocation>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<WeaponCollocation>>> GetWeaponCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsWeaponWeaponCollocation(last))
            .Get();

        HutaoResponse<List<WeaponCollocation>>? resp = await builder
            .SendAsync<HutaoResponse<List<WeaponCollocation>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsAvatarHoldingRate(last))
            .Get();

        HutaoResponse<List<AvatarConstellationInfo>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarConstellationInfo>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<TeamAppearance>>> GetTeamCombinationsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().StatisticsTeamCombination(last))
            .Get();

        HutaoResponse<List<TeamAppearance>>? resp = await builder
            .SendAsync<HutaoResponse<List<TeamAppearance>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<SimpleRecord?> GetPlayerRecordAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        IGameRecordClient gameRecordClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.User.IsOversea);

        Response<PlayerInfo> playerInfoResponse = await gameRecordClient
            .GetPlayerInfoAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (playerInfoResponse.IsOk())
        {
            Response<CharacterWrapper> charactersResponse = await gameRecordClient
                .GetCharacterListAsync(userAndUid, playerInfoResponse.Data, token)
                .ConfigureAwait(false);

            if (charactersResponse.IsOk())
            {
                Response<Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> spiralAbyssResponse = await gameRecordClient
                    .GetSpiralAbyssAsync(userAndUid, ScheduleType.Current, token)
                    .ConfigureAwait(false);

                if (spiralAbyssResponse.IsOk())
                {
                    HutaoUserOptions options = serviceProvider.GetRequiredService<HutaoUserOptions>();
                    return new(userAndUid.Uid.Value, charactersResponse.Data.List, spiralAbyssResponse.Data, options.GetActualUserName());
                }
            }
        }

        return default;
    }

    public async ValueTask<HutaoResponse> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RecordUpload())
            .PostJson(playerRecord);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.ViewModel.User;
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

/// <summary>
/// 胡桃API客户端
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoSpiralAbyssClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HutaoSpiralAbyssClient> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步检查对应的uid当前是否上传了数据
    /// GET /Record/CheckRecord/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>当前是否上传了数据</returns>
    public async ValueTask<HutaoResponse<bool>> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.RecordCheck(uid.Value))
            .Get();

        HutaoResponse<bool>? resp = await builder
            .SendAsync<HutaoResponse<bool>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取排行信息
    /// GET /Record/Rank/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>排行信息</returns>
    public async ValueTask<HutaoResponse<RankInfo>> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.RecordRank(uid.Value))
            .Get();

        HutaoResponse<RankInfo>? resp = await builder
            .SendAsync<HutaoResponse<RankInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<Overview>> GetOverviewAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsOverviewLast
            : HutaoEndpoints.StatisticsOverview;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<Overview>? resp = await builder
            .SendAsync<HutaoResponse<Overview>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsAvatarAttendanceRateLast
            : HutaoEndpoints.StatisticsAvatarAttendanceRate;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<List<AvatarAppearanceRank>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarAppearanceRank>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsAvatarUtilizationRateLast
            : HutaoEndpoints.StatisticsAvatarUtilizationRate;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<List<AvatarUsageRank>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarUsageRank>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarCollocation>>> GetAvatarCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsAvatarAvatarCollocationLast
            : HutaoEndpoints.StatisticsAvatarAvatarCollocation;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<List<AvatarCollocation>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarCollocation>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<WeaponCollocation>>> GetWeaponCollocationsAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsWeaponWeaponCollocationLast
            : HutaoEndpoints.StatisticsWeaponWeaponCollocation;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<List<WeaponCollocation>>? resp = await builder
            .SendAsync<HutaoResponse<List<WeaponCollocation>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsAvatarHoldingRateLast
            : HutaoEndpoints.StatisticsAvatarHoldingRate;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<List<AvatarConstellationInfo>>? resp = await builder
            .SendAsync<HutaoResponse<List<AvatarConstellationInfo>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<HutaoResponse<List<TeamAppearance>>> GetTeamCombinationsAsync(bool last = false, CancellationToken token = default)
    {
        string url = last
            ? HutaoEndpoints.StatisticsTeamCombinationLast
            : HutaoEndpoints.StatisticsTeamCombination;

        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(url)
            .Get();

        HutaoResponse<List<TeamAppearance>>? resp = await builder
            .SendAsync<HutaoResponse<List<TeamAppearance>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色的深渊记录
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家记录</returns>
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
                .GetCharactersAsync(userAndUid, playerInfoResponse.Data, token)
                .ConfigureAwait(false);

            if (charactersResponse.IsOk())
            {
                Response<Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss> spiralAbyssResponse = await gameRecordClient
                    .GetSpiralAbyssAsync(userAndUid, SpiralAbyssSchedule.Current, token)
                    .ConfigureAwait(false);

                if (spiralAbyssResponse.IsOk())
                {
                    HutaoUserOptions options = serviceProvider.GetRequiredService<HutaoUserOptions>();
                    return new(userAndUid.Uid.Value, charactersResponse.Data.Avatars, spiralAbyssResponse.Data, options.GetActualUserName());
                }
            }
        }

        return default;
    }

    /// <summary>
    /// 异步上传记录
    /// POST /Record/Upload
    /// </summary>
    /// <param name="playerRecord">玩家记录</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    public async ValueTask<HutaoResponse> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.RecordUpload)
            .PostJson(playerRecord);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}
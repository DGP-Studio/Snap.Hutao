// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
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
internal sealed partial class HomaSpiralAbyssClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<HomaSpiralAbyssClient> logger;
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
            .TryCatchSendAsync<HutaoResponse<bool>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
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
            .TryCatchSendAsync<HutaoResponse<RankInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取总览数据
    /// GET /Statistics/Overview
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>总览信息</returns>
    public async ValueTask<HutaoResponse<Overview>> GetOverviewAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsOverview)
            .Get();

        HutaoResponse<Overview>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<Overview>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色出场率
    /// GET /Statistics/Avatar/AttendanceRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async ValueTask<HutaoResponse<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsAvatarAttendanceRate)
            .Get();

        HutaoResponse<List<AvatarAppearanceRank>>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<List<AvatarAppearanceRank>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色使用率
    /// GET /Statistics/Avatar/UtilizationRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async ValueTask<HutaoResponse<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsAvatarUtilizationRate)
            .Get();

        HutaoResponse<List<AvatarUsageRank>>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<List<AvatarUsageRank>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色/武器/圣遗物搭配
    /// GET /Statistics/Avatar/AvatarCollocation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色/武器/圣遗物搭配</returns>
    public async ValueTask<HutaoResponse<List<AvatarCollocation>>> GetAvatarCollocationsAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsAvatarAvatarCollocation)
            .Get();

        HutaoResponse<List<AvatarCollocation>>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<List<AvatarCollocation>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取武器搭配
    /// GET /Statistics/Avatar/AvatarCollocation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色/武器/圣遗物搭配</returns>
    public async ValueTask<HutaoResponse<List<WeaponCollocation>>> GetWeaponCollocationsAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsWeaponWeaponCollocation)
            .Get();

        HutaoResponse<List<WeaponCollocation>>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<List<WeaponCollocation>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色命座信息
    /// GET /Statistics/Avatar/HoldingRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async ValueTask<HutaoResponse<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsAvatarHoldingRate)
            .Get();

        HutaoResponse<List<AvatarConstellationInfo>>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<List<AvatarConstellationInfo>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// GET /Team/Combination
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async ValueTask<HutaoResponse<List<TeamAppearance>>> GetTeamCombinationsAsync(CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(HutaoEndpoints.StatisticsTeamCombination)
            .Get();

        HutaoResponse<List<TeamAppearance>>? resp = await builder
            .TryCatchSendAsync<HutaoResponse<List<TeamAppearance>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
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
                    return new(userAndUid.Uid.Value, charactersResponse.Data.Avatars, spiralAbyssResponse.Data, options.ActualUserName);
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
            .TryCatchSendAsync<HutaoResponse>(httpClient, logger, token)
            .ConfigureAwait(false);

        return HutaoResponse.DefaultIfNull(resp);
    }
}
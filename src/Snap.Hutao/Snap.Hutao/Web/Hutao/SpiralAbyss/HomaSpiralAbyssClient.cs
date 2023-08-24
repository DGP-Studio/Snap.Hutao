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
    private readonly ILogger<HomaSpiralAbyssClient> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly JsonSerializerOptions options;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步检查对应的uid当前是否上传了数据
    /// GET /Record/CheckRecord/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>当前是否上传了数据</returns>
    public async ValueTask<Response<bool>> CheckRecordUploadedAsync(PlayerUid uid, CancellationToken token = default)
    {
        Response<bool>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<bool>>(HutaoEndpoints.RecordCheck(uid.Value), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取排行信息
    /// GET /Record/Rank/{Uid}
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>排行信息</returns>
    public async ValueTask<Response<RankInfo>> GetRankAsync(PlayerUid uid, CancellationToken token = default)
    {
        Response<RankInfo>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<RankInfo>>(HutaoEndpoints.RecordRank(uid.Value), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取总览数据
    /// GET /Statistics/Overview
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>总览信息</returns>
    public async ValueTask<Response<Overview>> GetOverviewAsync(CancellationToken token = default)
    {
        Response<Overview>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<Overview>>(HutaoEndpoints.StatisticsOverview, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色出场率
    /// GET /Statistics/Avatar/AttendanceRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async ValueTask<Response<List<AvatarAppearanceRank>>> GetAvatarAttendanceRatesAsync(CancellationToken token = default)
    {
        Response<List<AvatarAppearanceRank>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<AvatarAppearanceRank>>>(HutaoEndpoints.StatisticsAvatarAttendanceRate, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色使用率
    /// GET /Statistics/Avatar/UtilizationRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async ValueTask<Response<List<AvatarUsageRank>>> GetAvatarUtilizationRatesAsync(CancellationToken token = default)
    {
        Response<List<AvatarUsageRank>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<AvatarUsageRank>>>(HutaoEndpoints.StatisticsAvatarUtilizationRate, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色/武器/圣遗物搭配
    /// GET /Statistics/Avatar/AvatarCollocation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色/武器/圣遗物搭配</returns>
    public async ValueTask<Response<List<AvatarCollocation>>> GetAvatarCollocationsAsync(CancellationToken token = default)
    {
        Response<List<AvatarCollocation>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<AvatarCollocation>>>(HutaoEndpoints.StatisticsAvatarAvatarCollocation, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取武器搭配
    /// GET /Statistics/Avatar/AvatarCollocation
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色/武器/圣遗物搭配</returns>
    public async ValueTask<Response<List<WeaponCollocation>>> GetWeaponCollocationsAsync(CancellationToken token = default)
    {
        Response<List<WeaponCollocation>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<WeaponCollocation>>>(HutaoEndpoints.StatisticsWeaponWeaponCollocation, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色命座信息
    /// GET /Statistics/Avatar/HoldingRate
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async ValueTask<Response<List<AvatarConstellationInfo>>> GetAvatarHoldingRatesAsync(CancellationToken token = default)
    {
        Response<List<AvatarConstellationInfo>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<AvatarConstellationInfo>>>(HutaoEndpoints.StatisticsAvatarHoldingRate, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// GET /Team/Combination
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async ValueTask<Response<List<TeamAppearance>>> GetTeamCombinationsAsync(CancellationToken token = default)
    {
        Response<List<TeamAppearance>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<List<TeamAppearance>>>(HutaoEndpoints.StatisticsTeamCombination, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
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

        return null;
    }

    /// <summary>
    /// 异步上传记录
    /// POST /Record/Upload
    /// </summary>
    /// <param name="playerRecord">玩家记录</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    public async ValueTask<Response<string>> UploadRecordAsync(SimpleRecord playerRecord, CancellationToken token = default)
    {
        Response<string>? resp = await httpClient
            .TryCatchPostAsJsonAsync<SimpleRecord, Response<string>>(HutaoEndpoints.RecordUpload, playerRecord, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
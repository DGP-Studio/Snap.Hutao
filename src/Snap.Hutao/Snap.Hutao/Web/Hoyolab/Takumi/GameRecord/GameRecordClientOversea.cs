// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// Hoyoverse game record provider
/// </summary>
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc3)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class GameRecordClientOversea : IGameRecordClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<GameRecordClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取实时便笺
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实时便笺</returns>
    [ApiInformation(Cookie = CookieType.Cookie, Salt = SaltType.OSX4)]
    public async ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordDailyNote(userAndUid.Uid))
            .SetUserCookie(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<DailyNote.DailyNote>? resp = await builder
            .TryCatchSendAsync<Response<DailyNote.DailyNote>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家基础信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家的基础信息</returns>
    [ApiInformation(Cookie = CookieType.LToken, Salt = SaltType.OSX4)]
    public async ValueTask<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordIndex(userAndUid.Uid))
            .SetUserCookie(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<PlayerInfo>? resp = await builder
            .TryCatchSendAsync<Response<PlayerInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家深渊信息
    /// </summary>
    /// <param name="userAndUid">用户</param>
    /// <param name="schedule">1：当期，2：上期</param>
    /// <param name="token">取消令牌</param>
    /// <returns>深渊信息</returns>
    [ApiInformation(Cookie = CookieType.Cookie, Salt = SaltType.OSX4)]
    public async ValueTask<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
            .SetUserCookie(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<SpiralAbyss.SpiralAbyss>? resp = await builder
            .TryCatchSendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家角色详细信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="playerInfo">玩家的基础信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    [ApiInformation(Cookie = CookieType.LToken, Salt = SaltType.OSX4)]
    public async ValueTask<Response<CharacterWrapper>> GetCharactersAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordCharacter)
            .SetUserCookie(userAndUid, CookieType.Cookie)
            .PostJson(new CharacterData(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id)));

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<CharacterWrapper>? resp = await builder
            .TryCatchSendAsync<Response<CharacterWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
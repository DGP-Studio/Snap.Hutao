// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 游戏记录提供器
/// </summary>
[HighQuality]
[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class GameRecordClient : IGameRecordClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<GameRecordClient> logger;
    private readonly HttpClient httpClient;

    /// <summary>
    /// 异步获取实时便笺
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实时便笺</returns>
    [ApiInformation(Cookie = CookieType.Cookie, Salt = SaltType.X4)]
    public async ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordDailyNote(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<DailyNote.DailyNote>? resp = await builder
            .TryCatchSendAsync<Response<DailyNote.DailyNote>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebDailyNoteVerificationFailed;
            IGeetestCardVerifier verifier = serviceProvider.GetRequiredService<HomaGeetestCardVerifier>();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordDailyNote(userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetXrpcChallenge(challenge)
                    .Get();

                await verifiedbuilder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedbuilder
                    .TryCatchSendAsync<Response<DailyNote.DailyNote>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家基础信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家的基础信息</returns>
    [ApiInformation(Cookie = CookieType.LToken, Salt = SaltType.X4)]
    public async ValueTask<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordIndex(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<PlayerInfo>? resp = await builder
            .TryCatchSendAsync<Response<PlayerInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode == (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;
            IGeetestCardVerifier verifier = serviceProvider.GetRequiredService<HomaGeetestCardVerifier>();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordIndex(userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetXrpcChallenge(challenge)
                    .Get();

                await verifiedbuilder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedbuilder
                    .TryCatchSendAsync<Response<PlayerInfo>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家深渊信息
    /// </summary>
    /// <param name="userAndUid">用户</param>
    /// <param name="schedule">1：当期，2：上期</param>
    /// <param name="token">取消令牌</param>
    /// <returns>深渊信息</returns>
    [ApiInformation(Cookie = CookieType.Cookie, Salt = SaltType.X4)]
    public async ValueTask<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<SpiralAbyss.SpiralAbyss>? resp = await builder
            .TryCatchSendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode == (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;
            IGeetestCardVerifier verifier = serviceProvider.GetRequiredService<HomaGeetestCardVerifier>();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetXrpcChallenge(challenge)
                    .Get();

                await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await builder
                    .TryCatchSendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取角色基本信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色基本信息</returns>
    [ApiInformation(Cookie = CookieType.LToken, Salt = SaltType.X4)]
    public async ValueTask<Response<BasicRoleInfo>> GetRoleBasicInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordRoleBasicInfo(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<BasicRoleInfo>? resp = await builder
            .TryCatchSendAsync<Response<BasicRoleInfo>>(httpClient, logger, token)
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
    [ApiInformation(Cookie = CookieType.LToken, Salt = SaltType.X4)]
    public async ValueTask<Response<CharacterWrapper>> GetCharactersAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordCharacter)
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .PostJson(new CharacterData(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id)));

        await builder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<CharacterWrapper>? resp = await builder
            .TryCatchSendAsync<Response<CharacterWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode == (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;
            IGeetestCardVerifier verifier = serviceProvider.GetRequiredService<HomaGeetestCardVerifier>();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedBuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordCharacter)
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetXrpcChallenge(challenge)
                    .PostJson(new CharacterData(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id)));

                await verifiedBuilder.SetDynamicSecretAsync(DynamicSecretVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedBuilder
                    .TryCatchSendAsync<Response<CharacterWrapper>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }
}
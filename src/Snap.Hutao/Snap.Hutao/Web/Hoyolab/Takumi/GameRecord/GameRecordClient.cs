// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Verification;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

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

    public async ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordDailyNote(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<DailyNote.DailyNote>? resp = await builder
            .SendAsync<Response<DailyNote.DailyNote>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebDailyNoteVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForDailyNote();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordDailyNote(userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
                    .SetXrpcChallenge(challenge)
                    .Get();

                await verifiedbuilder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedbuilder
                    .SendAsync<Response<DailyNote.DailyNote>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordIndex(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<PlayerInfo>? resp = await builder
            .SendAsync<Response<PlayerInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode == (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForIndex();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordIndex(userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
                    .SetXrpcChallenge(challenge)
                    .Get();

                await verifiedbuilder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedbuilder
                    .SendAsync<Response<PlayerInfo>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, ScheduleType schedule, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<SpiralAbyss.SpiralAbyss>? resp = await builder
            .SendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode == (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForSpiralAbyss();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
                    .SetXrpcChallenge(challenge)
                    .Get();

                await verifiedbuilder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedbuilder
                    .SendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<BasicRoleInfo>> GetRoleBasicInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordRoleBasicInfo(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<BasicRoleInfo>? resp = await builder
            .SendAsync<Response<BasicRoleInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<CharacterWrapper>> GetCharactersAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordCharacter)
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .PostJson(new CharacterData(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id)));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<CharacterWrapper>? resp = await builder
            .SendAsync<Response<CharacterWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForCharacter();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedBuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(ApiEndpoints.GameRecordCharacter)
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
                    .SetXrpcChallenge(challenge)
                    .PostJson(new CharacterData(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id)));

                await verifiedBuilder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedBuilder
                    .SendAsync<Response<CharacterWrapper>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<RoleCombat.RoleCombat>> GetRoleCombatAsync(UserAndUid userAndUid, CancellationToken token = default(CancellationToken))
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiEndpoints.GameRecordRoleCombat(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(ApiEndpoints.WebStaticMihoyoReferer)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<RoleCombat.RoleCombat>? resp = await builder
            .SendAsync<Response<RoleCombat.RoleCombat>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForRoleCombat();

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                builder.Resurrect().SetXrpcChallenge(challenge);
                await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await builder
                    .SendAsync<Response<RoleCombat.RoleCombat>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }
}
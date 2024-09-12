// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Endpoint.Hoyolab;
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
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordDailyNote(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
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
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForDailyNote(apiEndpoints);

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(apiEndpoints.GameRecordDailyNote(userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(apiEndpoints.WebStaticReferer())
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
            .SetRequestUri(apiEndpoints.GameRecordIndex(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
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
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForIndex(apiEndpoints);

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(apiEndpoints.GameRecordIndex(userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(apiEndpoints.WebStaticReferer())
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
            .SetRequestUri(apiEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
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
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForSpiralAbyss(apiEndpoints);

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedbuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(apiEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(apiEndpoints.WebStaticReferer())
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
            .SetRequestUri(apiEndpoints.GameRecordRoleBasicInfo(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<BasicRoleInfo>? resp = await builder
            .SendAsync<Response<BasicRoleInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<Character>>> GetCharacterListAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordCharacterList())
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .PostJson(new CharacterData(userAndUid.Uid));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<ListWrapper<Character>>? resp = await builder
            .SendAsync<Response<ListWrapper<Character>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForCharacterAll(apiEndpoints);

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedBuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(apiEndpoints.GameRecordCharacterList())
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(apiEndpoints.WebStaticReferer())
                    .SetXrpcChallenge(challenge)
                    .PostJson(new CharacterData(userAndUid.Uid));

                await verifiedBuilder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedBuilder
                    .SendAsync<Response<ListWrapper<Character>>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<ListWrapper<DetailedCharacter>>> GetCharacterDetailAsync(UserAndUid userAndUid, List<AvatarId> characterIds, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordCharacterDetail())
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .PostJson(new CharacterData(userAndUid.Uid, characterIds));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<ListWrapper<DetailedCharacter>>? resp = await builder
            .SendAsync<Response<ListWrapper<DetailedCharacter>>>(httpClient, logger, token)
            .ConfigureAwait(false);

        // We have a verification procedure to handle
        if (resp?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            resp.Message = SH.WebIndexOrSpiralAbyssVerificationFailed;

            IGeetestCardVerifier verifier = serviceProvider.GetRequiredKeyedService<IGeetestCardVerifier>(GeetestCardVerifierType.Custom);
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForCharacterDetail(apiEndpoints);

            if (await verifier.TryValidateXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                HttpRequestMessageBuilder verifiedBuilder = httpRequestMessageBuilderFactory.Create()
                    .SetRequestUri(apiEndpoints.GameRecordCharacterDetail())
                    .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
                    .SetReferer(apiEndpoints.WebStaticReferer())
                    .SetXrpcChallenge(challenge)
                    .PostJson(new CharacterData(userAndUid.Uid, characterIds));

                await verifiedBuilder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

                resp = await verifiedBuilder
                    .SendAsync<Response<ListWrapper<DetailedCharacter>>>(httpClient, logger, token)
                    .ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<RoleCombat.RoleCombat>> GetRoleCombatAsync(UserAndUid userAndUid, CancellationToken token = default(CancellationToken))
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordRoleCombat(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
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
            CardVerifiationHeaders headers = CardVerifiationHeaders.CreateForRoleCombat(apiEndpoints);

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
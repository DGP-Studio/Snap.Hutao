// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Geetest;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Endpoint.Hoyolab;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

[HttpClient(HttpClientConfiguration.XRpc)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class GameRecordClient : IGameRecordClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IServiceProvider serviceProvider;
    [FromKeyed(ApiEndpointsKind.Chinese)]
    private readonly IApiEndpoints apiEndpoints;
    private readonly HttpClient httpClient;

    [GeneratedConstructor]
    public partial GameRecordClient(IServiceProvider serviceProvider, HttpClient httpClient);

    public async ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordDailyNote(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .SetHeader("x-rpc-tool_verison", "v5.0.1-ys")
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<DailyNote.DailyNote>? resp = await builder
            .SendAsync<Response<DailyNote.DailyNote>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebDailyNoteVerificationFailed, CardVerifiationHeaders.CreateForDailyNote, token).ConfigureAwait(false);
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
            .SendAsync<Response<PlayerInfo>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForIndex, token).ConfigureAwait(false);
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
            .SendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForSpiralAbyss, token).ConfigureAwait(false);
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
            .SendAsync<Response<BasicRoleInfo>>(httpClient, token)
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
            .SendAsync<Response<ListWrapper<Character>>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForCharacterAll, token).ConfigureAwait(false);
    }

    public async ValueTask<Response<ListWrapper<DetailedCharacter>>> GetCharacterDetailAsync(UserAndUid userAndUid, ImmutableArray<AvatarId> characterIds, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordCharacterDetail())
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .PostJson(new CharacterData(userAndUid.Uid, characterIds));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<ListWrapper<DetailedCharacter>>? resp = await builder
            .SendAsync<Response<ListWrapper<DetailedCharacter>>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForCharacterDetail, token).ConfigureAwait(false);
    }

    public async ValueTask<Response<RoleCombat.RoleCombat>> GetRoleCombatAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordRoleCombat(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<RoleCombat.RoleCombat>? resp = await builder
            .SendAsync<Response<RoleCombat.RoleCombat>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForRoleCombat, token).ConfigureAwait(false);
    }

    public async ValueTask<Response<HardChallenge.HardChallenge>> GetHardChallengeAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordHardChallenge(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<HardChallenge.HardChallenge>? resp = await builder
            .SendAsync<Response<HardChallenge.HardChallenge>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForHardChallenge, token).ConfigureAwait(false);
    }

    public async ValueTask<Response<HardChallengePopularity>> GetHardChallengePopularityAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(apiEndpoints.GameRecordHardChallengePopularity(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .SetReferer(apiEndpoints.WebStaticReferer())
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);

        Response<HardChallengePopularity>? resp = await builder
            .SendAsync<Response<HardChallengePopularity>>(httpClient, token)
            .ConfigureAwait(false);

        return await RetryIf1034Async(builder, userAndUid, resp, SH.WebIndexOrSpiralAbyssVerificationFailed, CardVerifiationHeaders.CreateForHardChallenge, token).ConfigureAwait(false);
    }

    private async ValueTask<TResponse> RetryIf1034Async<TResponse>(HttpRequestMessageBuilder builder, UserAndUid userAndUid, TResponse? response, string message, Func<IApiEndpoints, CardVerifiationHeaders> headersFactory, CancellationToken token = default)
        where TResponse : class, ICommonResponse<TResponse>
    {
        // We have a verification procedure to handle
        if (response?.ReturnCode is (int)KnownReturnCode.CODE1034)
        {
            // Replace message
            response.Message = message;

            IGeetestService geetestService = serviceProvider.GetRequiredService<IGeetestService>();
            CardVerifiationHeaders headers = headersFactory(apiEndpoints);

            if (await geetestService.TryVerifyXrpcChallengeAsync(userAndUid.User, headers, token).ConfigureAwait(false) is { } challenge)
            {
                builder.Resurrect().SetXrpcChallenge(challenge);
                await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.X4, false).ConfigureAwait(false);
                response = await builder.SendAsync<TResponse>(httpClient, token).ConfigureAwait(false);
            }
        }

        return Response.Response.DefaultIfNull(response);
    }
}
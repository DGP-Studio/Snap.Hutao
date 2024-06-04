// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.DataSigning;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.XRpc3)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed partial class GameRecordClientOversea : IGameRecordClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ILogger<GameRecordClient> logger;
    private readonly HttpClient httpClient;

    public async ValueTask<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordDailyNote(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<DailyNote.DailyNote>? resp = await builder
            .SendAsync<Response<DailyNote.DailyNote>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordIndex(userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<PlayerInfo>? resp = await builder
            .SendAsync<Response<PlayerInfo>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid))
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .Get();

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<SpiralAbyss.SpiralAbyss>? resp = await builder
            .SendAsync<Response<SpiralAbyss.SpiralAbyss>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<Response<CharacterWrapper>> GetCharactersAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(ApiOsEndpoints.GameRecordCharacter)
            .SetUserCookieAndFpHeader(userAndUid, CookieType.Cookie)
            .PostJson(new CharacterData(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id)));

        await builder.SignDataAsync(DataSignAlgorithmVersion.Gen2, SaltType.OSX4, false).ConfigureAwait(false);

        Response<CharacterWrapper>? resp = await builder
            .SendAsync<Response<CharacterWrapper>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
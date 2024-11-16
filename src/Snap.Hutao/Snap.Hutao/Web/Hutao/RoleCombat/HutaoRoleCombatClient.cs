// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Endpoint.Hutao;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Hutao.RoleCombat.Post;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.RoleCombat;

[ConstructorGenerated(ResolveHttpClient = true)]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed partial class HutaoRoleCombatClient
{
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly IHutaoEndpointsFactory hutaoEndpointsFactory;
    private readonly ILogger<HutaoRoleCombatClient> logger;
    private readonly IServiceProvider serviceProvider;
    private readonly HttpClient httpClient;

    public async ValueTask<HutaoResponse<RoleCombatStatisticsItem>> GetStatisticsAsync(bool last = false, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RoleCombatStatistics(last))
            .Get();

        HutaoResponse<RoleCombatStatisticsItem>? resp = await builder
            .SendAsync<HutaoResponse<RoleCombatStatisticsItem>>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }

    public async ValueTask<SimpleRoleCombatRecord?> GetPlayerRecordAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        IGameRecordClient gameRecordClient = serviceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.User.IsOversea);

        // Reduce risk verify chance.
        Response<PlayerInfo> playerInfoResponse = await gameRecordClient
            .GetPlayerInfoAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(playerInfoResponse, serviceProvider))
        {
            return default;
        }

        Response<Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat> roleCombatResponse = await gameRecordClient
            .GetRoleCombatAsync(userAndUid, token)
            .ConfigureAwait(false);

        if (!ResponseValidator.TryValidate(roleCombatResponse, serviceProvider, out Hoyolab.Takumi.GameRecord.RoleCombat.RoleCombat? roleCombat))
        {
            return default;
        }

        if (roleCombat.Data.FirstOrDefault() is { HasData: true } data)
        {
            return new(userAndUid.Uid.Value, data.Detail.BackupAvatars.SelectList(a => a.AvatarId.Value), data.Schedule.ScheduleId.Value);
        }

        return default;
    }

    public async ValueTask<HutaoResponse> UploadRecordAsync(SimpleRoleCombatRecord record, CancellationToken token = default)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create()
            .SetRequestUri(hutaoEndpointsFactory.Create().RoleCombatRecordUpload())
            .PostJson(record);

        HutaoResponse? resp = await builder
            .SendAsync<HutaoResponse>(httpClient, logger, token)
            .ConfigureAwait(false);

        return Web.Response.Response.DefaultIfNull(resp);
    }
}
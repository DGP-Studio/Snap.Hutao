// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.Hutao;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

[Service(ServiceLifetime.Scoped, typeof(IGachaLogHutaoCloudService))]
internal sealed partial class GachaLogHutaoCloudService : IGachaLogHutaoCloudService
{
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly HutaoUserOptions hutaoUserOptions;
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial GachaLogHutaoCloudService(IServiceProvider serviceProvider);

    public async ValueTask<HutaoResponse<ImmutableArray<GachaEntry>>> GetGachaEntriesAsync(CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await hutaoUserOptions.GetAccessTokenAsync(token).ConfigureAwait(false);
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            return await homaGachaLogClient.GetGachaEntriesAsync(accessToken, token);
        }
    }

    public async ValueTask<ValueResult<bool, string>> UploadGachaItemsAsync(GachaArchive gachaArchive, CancellationToken token = default)
    {
        string uid = gachaArchive.Uid;
        if (await GetNewestEndIdsFromCloudAsync(uid, token).ConfigureAwait(false) is { } endIds)
        {
            List<Web.Hutao.GachaLog.GachaItem> items = [];
            foreach ((GachaType type, long endId) in endIds)
            {
                items.AddRange(gachaLogRepository.GetHutaoGachaItemListByArchiveIdAndQueryTypeNewerThanEndId(gachaArchive.InnerId, type, endId));
            }

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                string? accessToken = await hutaoUserOptions.GetAccessTokenAsync(token).ConfigureAwait(false);
                HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
                return await homaGachaLogClient.UploadGachaItemsAsync(accessToken, uid, items, token).ConfigureAwait(false);
            }
        }

        return new(false, SH.ServiceGachaLogHutaoCloudEndIdFetchFailed);
    }

    public async ValueTask<ValueResult<bool, Guid>> RetrieveGachaArchiveIdAsync(string uid, CancellationToken token = default)
    {
        GachaArchive? archive = gachaLogRepository.GetGachaArchiveByUid(uid);
        EndIds endIds = CreateEndIdsForArchive(archive);

        ImmutableArray<Web.Hutao.GachaLog.GachaItem> array;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await hutaoUserOptions.GetAccessTokenAsync(token).ConfigureAwait(false);
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            Response<ImmutableArray<Web.Hutao.GachaLog.GachaItem>> resp = await homaGachaLogClient.RetrieveGachaItemsAsync(accessToken, uid, endIds, token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(resp, scope.ServiceProvider, out array))
            {
                return new(false, default);
            }
        }

        if (archive is null)
        {
            archive = GachaArchive.Create(uid);
            gachaLogRepository.AddGachaArchive(archive);
        }

        Guid archiveId = archive.InnerId;
        gachaLogRepository.AddGachaItemRange(array.SelectAsArray(static (i, archiveId) => Model.Entity.GachaItem.From(archiveId, i), archiveId));
        return new(true, archive.InnerId);
    }

    public async ValueTask<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await hutaoUserOptions.GetAccessTokenAsync(token).ConfigureAwait(false);
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            return await homaGachaLogClient.DeleteGachaItemsAsync(accessToken, uid, token).ConfigureAwait(false);
        }
    }

    public async ValueTask<ValueResult<bool, HutaoStatistics>> GetCurrentEventStatisticsAsync(CancellationToken token = default)
    {
        GachaEventStatistics? raw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await hutaoUserOptions.GetAccessTokenAsync(token).ConfigureAwait(false);
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            Response<GachaEventStatistics> response = await homaGachaLogClient.GetGachaEventStatisticsAsync(accessToken, token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, scope.ServiceProvider, out raw))
            {
                return new(false, default!);
            }
        }

        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return new(false, default!);
        }

        HutaoStatisticsFactoryMetadataContext context = await metadataService
            .GetContextAsync<HutaoStatisticsFactoryMetadataContext>(token)
            .ConfigureAwait(false);

        try
        {
            HutaoStatisticsFactory factory = new(context);
            HutaoStatistics statistics = factory.Create(raw);
            return new(true, statistics);
        }
        catch
        {
            // 元数据未能即时更新导致异常
            return new(false, default!);
        }
    }

    private async ValueTask<EndIds?> GetNewestEndIdsFromCloudAsync(string uid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await hutaoUserOptions.GetAccessTokenAsync(token).ConfigureAwait(false);
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            Response<EndIds> resp = await homaGachaLogClient.GetEndIdsAsync(accessToken, uid, token).ConfigureAwait(false);
            ResponseValidator.TryValidate(resp, scope.ServiceProvider, out EndIds? raw);
            return raw;
        }
    }

    private EndIds CreateEndIdsForArchive(GachaArchive? archive)
    {
        EndIds endIds = new();
        foreach (GachaType type in GachaLog.QueryTypes)
        {
            if (archive is not null)
            {
                endIds[type] = gachaLogRepository.GetOldestGachaItemIdByArchiveIdAndQueryType(archive.InnerId, type);
            }
        }

        return endIds;
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录胡桃云服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaLogHutaoCloudService))]
internal sealed partial class GachaLogHutaoCloudService : IGachaLogHutaoCloudService
{
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;

    /// <inheritdoc/>
    public ValueTask<HutaoResponse<List<GachaEntry>>> GetGachaEntriesAsync(CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            return homaGachaLogClient.GetGachaEntriesAsync(token);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> UploadGachaItemsAsync(GachaArchive gachaArchive, CancellationToken token = default)
    {
        string uid = gachaArchive.Uid;
        if (await GetEndIdsFromCloudAsync(uid, token).ConfigureAwait(false) is { } endIds)
        {
            List<Web.Hutao.GachaLog.GachaItem> items = [];
            foreach ((GachaType type, long endId) in endIds)
            {
                items.AddRange(gachaLogRepository.GetHutaoGachaItemListByArchiveIdAndQueryTypeNewerThanEndId(gachaArchive.InnerId, type, endId));
            }

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
                return await homaGachaLogClient.UploadGachaItemsAsync(uid, items, token).ConfigureAwait(false);
            }
        }

        return new(false, SH.ServiceGachaLogHutaoCloudEndIdFetchFailed);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, Guid>> RetrieveGachaArchiveIdAsync(string uid, CancellationToken token = default)
    {
        GachaArchive? archive = gachaLogRepository.GetGachaArchiveByUid(uid);
        EndIds endIds = CreateEndIds(archive);

        List<Web.Hutao.GachaLog.GachaItem>? list;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            Response<List<Web.Hutao.GachaLog.GachaItem>> resp = await homaGachaLogClient.RetrieveGachaItemsAsync(uid, endIds, token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(resp, serviceProvider, out list))
            {
                return new(false, default);
            }
        }

        if (archive is null)
        {
            archive = GachaArchive.From(uid);
            gachaLogRepository.AddGachaArchive(archive);
        }

        Guid archiveId = archive.InnerId;
        List<Model.Entity.GachaItem> gachaItems = list.SelectList(i => Model.Entity.GachaItem.From(archiveId, i));
        gachaLogRepository.AddGachaItemRange(gachaItems);
        return new(true, archive.InnerId);
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            return await homaGachaLogClient.DeleteGachaItemsAsync(uid, token).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ValueResult<bool, HutaoStatistics>> GetCurrentEventStatisticsAsync(CancellationToken token = default)
    {
        GachaEventStatistics? raw;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            Response<GachaEventStatistics> response = await homaGachaLogClient.GetGachaEventStatisticsAsync(token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidate(response, serviceProvider, out raw))
            {
                return new(false, default!);
            }
        }

        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
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
                // 元数据未能即时更新导致异常？
                return new(false, default!);
            }
        }

        return new(false, default!);
    }

    private async ValueTask<EndIds?> GetEndIdsFromCloudAsync(string uid, CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            HomaGachaLogClient homaGachaLogClient = scope.ServiceProvider.GetRequiredService<HomaGachaLogClient>();
            Response<EndIds> resp = await homaGachaLogClient.GetEndIdsAsync(uid, token).ConfigureAwait(false);
            ResponseValidator.TryValidate(resp, serviceProvider, out EndIds? raw);
            return raw;
        }
    }

    private EndIds CreateEndIds(GachaArchive? archive)
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
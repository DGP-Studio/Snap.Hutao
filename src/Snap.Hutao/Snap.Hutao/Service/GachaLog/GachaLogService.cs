// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Metadata.ContextAbstraction;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Service.GachaLog;

[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaLogService))]
internal sealed partial class GachaLogService : IGachaLogService
{
    private readonly IGachaStatisticsSlimFactory gachaStatisticsSlimFactory;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly IUIGFExportService gachaLogExportService;
    private readonly IUIGFImportService gachaLogImportService;
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ILogger<GachaLogService> logger;
    private readonly GachaInfoClient gachaInfoClient;
    private readonly ITaskContext taskContext;

    private GachaLogServiceMetadataContext context;
    private AdvancedDbCollectionView<GachaArchive>? archives;

    public AdvancedDbCollectionView<GachaArchive>? Archives
    {
        get => archives;
        private set => archives = value;
    }

    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
    {
        if (context is { IsInitialized: true })
        {
            return true;
        }

        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            context = await metadataService.GetContextAsync<GachaLogServiceMetadataContext>(token).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Archives = new(gachaLogDbService.GetGachaArchiveCollection(), serviceProvider);
            return true;
        }
        else
        {
            return false;
        }
    }

    public async ValueTask<GachaStatistics> GetStatisticsAsync(GachaArchive archive)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            List<GachaItem> items = gachaLogDbService.GetGachaItemListByArchiveId(archive.InnerId);
            return await gachaStatisticsFactory.CreateAsync(items, context).ConfigureAwait(false);
        }
    }

    public async ValueTask<List<GachaStatisticsSlim>> GetStatisticsSlimListAsync(CancellationToken token = default)
    {
        await InitializeAsync(token).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(Archives);

        List<GachaStatisticsSlim> statistics = [];
        foreach (GachaArchive archive in Archives)
        {
            List<GachaItem> items = gachaLogDbService.GetGachaItemListByArchiveId(archive.InnerId);
            GachaStatisticsSlim slim = await gachaStatisticsSlimFactory.CreateAsync(context, items, archive.Uid).ConfigureAwait(false);
            statistics.Add(slim);
        }

        return statistics;
    }

    public ValueTask<UIGF> ExportToUIGFAsync(GachaArchive archive)
    {
        return gachaLogExportService.ExportAsync(context, archive);
    }

    public async ValueTask ImportFromUIGFAsync(UIGF uigf)
    {
        ArgumentNullException.ThrowIfNull(Archives);
        await gachaLogImportService.ImportAsync(context, uigf, Archives).ConfigureAwait(false);
    }

    public async ValueTask<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategyKind kind, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        bool isLazy = kind switch
        {
            RefreshStrategyKind.AggressiveMerge => false,
            RefreshStrategyKind.LazyMerge => true,
            _ => throw HutaoException.NotSupported(),
        };

        (bool authkeyValid, GachaArchive? target) = await FetchGachaLogsAsync(query, isLazy, progress, token).ConfigureAwait(false);

        if (target is not null && Archives is not null)
        {
            await taskContext.SwitchToMainThreadAsync();
            Archives.CurrentItem = target;
        }

        return authkeyValid;
    }

    public async ValueTask RemoveArchiveAsync(GachaArchive archive)
    {
        ArgumentNullException.ThrowIfNull(archives);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        gachaLogDbService.RemoveGachaArchiveById(archive.InnerId);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archives.Remove(archive);
    }

    public async ValueTask<GachaArchive> EnsureArchiveInCollectionAsync(Guid archiveId, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(Archives);

        if (Archives.SourceCollection.SingleOrDefault(a => a.InnerId == archiveId) is { } archive)
        {
            return archive;
        }
        else
        {
            GachaArchive? newArchive = gachaLogDbService.GetGachaArchiveById(archiveId);
            ArgumentNullException.ThrowIfNull(newArchive);

            await taskContext.SwitchToMainThreadAsync();
            Archives.Add(newArchive);
            return newArchive;
        }
    }

    private async ValueTask<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(Archives);

        GachaLogFetchContext fetchContext = new(gachaLogDbService, taskContext, context, isLazy);

        foreach (GachaType configType in GachaLog.QueryTypes)
        {
            fetchContext.ResetForProcessingType(configType, query);

            do
            {
                Response<GachaLogPage> response = await gachaInfoClient
                    .GetGachaLogPageAsync(fetchContext.TypedQueryOptions, token)
                    .ConfigureAwait(false);

                if (!response.TryGetData(out GachaLogPage? page))
                {
                    fetchContext.Report(progress, isAuthKeyTimeout: true);
                    break;
                }

                List<GachaLogItem> items = page.List;
                fetchContext.ResetForProcessingPage();

                foreach (GachaLogItem item in items)
                {
                    fetchContext.EnsureArchiveAndEndId(item, Archives, gachaLogDbService);

                    if (fetchContext.ShouldAddItem(item))
                    {
                        fetchContext.AddItem(item);
                    }
                    else
                    {
                        fetchContext.CompleteAdding();
                        break;
                    }
                }

                fetchContext.Report(progress);

                if (fetchContext.ItemsHaveReachEnd(items))
                {
                    // exit current type fetch loop
                    break;
                }

                await Task.Delay(Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);
            }
            while (true);

            if (fetchContext.FetchStatus.AuthKeyTimeout)
            {
                break;
            }

            // save items for each queryType
            token.ThrowIfCancellationRequested();
            fetchContext.SaveItems();
            await Delay.RandomMilliSeconds(1000, 2000).ConfigureAwait(false);
        }

        return new(!fetchContext.FetchStatus.AuthKeyTimeout, fetchContext.TargetArchive);
    }
}
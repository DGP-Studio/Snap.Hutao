// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

[Service(ServiceLifetime.Scoped, typeof(IGachaLogService))]
internal sealed partial class GachaLogService : IGachaLogService
{
    private readonly IGachaStatisticsSlimFactory gachaStatisticsSlimFactory;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly IGachaLogRepository gachaLogRepository;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<GachaLogService> logger;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock archivesLock = new();
    private IAdvancedDbCollectionView<GachaArchive>? archives;

    [GeneratedConstructor]
    public partial GachaLogService(IServiceProvider serviceProvider);

    public async ValueTask<IAdvancedDbCollectionView<GachaArchive>> GetArchiveCollectionAsync()
    {
        using (await archivesLock.LockAsync().ConfigureAwait(false))
        {
            return archives ??= gachaLogRepository.GetGachaArchiveCollection().ToAdvancedDbCollectionView(serviceProvider);
        }
    }

    public async ValueTask<GachaStatistics> GetStatisticsAsync(GachaLogServiceMetadataContext context, GachaArchive archive)
    {
        using (ValueStopwatch.MeasureExecution(logger))
        {
            ImmutableArray<GachaItem> items = gachaLogRepository.GetGachaItemImmutableArrayByArchiveId(archive.InnerId);
            return await gachaStatisticsFactory.CreateAsync(context, items).ConfigureAwait(false);
        }
    }

    public async ValueTask<ImmutableArray<GachaStatisticsSlim>> GetStatisticsSlimImmutableArrayAsync(GachaLogServiceMetadataContext context, CancellationToken token = default)
    {
        ImmutableArray<GachaStatisticsSlim>.Builder statistics = ImmutableArray.CreateBuilder<GachaStatisticsSlim>();
        foreach (GachaArchive archive in await GetArchiveCollectionAsync().ConfigureAwait(false))
        {
            ImmutableArray<GachaItem> items = gachaLogRepository.GetGachaItemImmutableArrayByArchiveId(archive.InnerId);
            GachaStatisticsSlim slim = await gachaStatisticsSlimFactory.CreateAsync(context, items, archive.Uid).ConfigureAwait(false);
            statistics.Add(slim);
        }

        return statistics.ToImmutable();
    }

    public async ValueTask<bool> RefreshGachaLogAsync(GachaLogServiceMetadataContext context, GachaLogQuery query, RefreshStrategyKind kind, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        bool isLazy = kind switch
        {
            RefreshStrategyKind.AggressiveMerge => false,
            RefreshStrategyKind.LazyMerge => true,
            _ => throw HutaoException.NotSupported(),
        };

        (bool authkeyValid, GachaArchive? target) = await FetchGachaLogsAsync(context, query, isLazy, progress, token).ConfigureAwait(false);

        if (target is not null)
        {
            IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            localArchives.MoveCurrentTo(target);
        }

        return authkeyValid;
    }

    public async ValueTask RemoveArchiveAsync(GachaArchive archive)
    {
        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        gachaLogRepository.RemoveGachaArchiveById(archive.InnerId);

        // Sync cache
        IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        localArchives.Remove(archive);
        localArchives.MoveCurrentToFirst();
    }

    public async ValueTask<GachaArchive> EnsureArchiveInCollectionAsync(Guid archiveId, CancellationToken token = default)
    {
        IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
        if (localArchives.Source.SingleOrDefault(a => a.InnerId == archiveId) is { } archive)
        {
            return archive;
        }

        GachaArchive? newArchive = gachaLogRepository.GetGachaArchiveById(archiveId);
        ArgumentNullException.ThrowIfNull(newArchive);

        await taskContext.SwitchToMainThreadAsync();
        localArchives.Add(newArchive);
        return newArchive;
    }

    private async ValueTask<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogServiceMetadataContext context, GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        IAdvancedDbCollectionView<GachaArchive> localArchives = await GetArchiveCollectionAsync().ConfigureAwait(false);
        GachaLogFetchContext fetchContext = new(gachaLogRepository, context, isLazy);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            GachaInfoClient gachaInfoClient = scope.ServiceProvider.GetRequiredService<GachaInfoClient>();

            foreach (GachaType configType in GachaLog.QueryTypes)
            {
                fetchContext.ResetType(configType, query);

                do
                {
                    Response<GachaLogPage> response = await gachaInfoClient
                        .GetGachaLogPageAsync(fetchContext.TypedQueryOptions, token)
                        .ConfigureAwait(false);

                    // Fast break fetching if authkey timeout
                    if (!ResponseValidator.TryValidateWithoutUINotification(response, out GachaLogPage? page))
                    {
                        fetchContext.Report(progress, isAuthKeyTimeout: true);
                        break;
                    }

                    fetchContext.ResetCurrentPage();
                    ImmutableArray<GachaLogItem> items = page.List;

                    foreach (GachaLogItem item in items)
                    {
                        fetchContext.EnsureArchiveAndEndId(item, localArchives, gachaLogRepository);

                        if (fetchContext.ShouldAddItem(item))
                        {
                            fetchContext.AddItem(item);
                        }
                        else
                        {
                            fetchContext.CompleteCurrentTypeAdding();
                            break;
                        }
                    }

                    fetchContext.Report(progress);
                    await Task.Delay(Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);

                    if (fetchContext.HasReachCurrentTypeEnd(items))
                    {
                        // Exit current type fetch loop
                        break;
                    }
                }
                while (true);

                // Fast break query type loop if authkey timeout, skip saving items
                if (fetchContext.Status.AuthKeyTimeout)
                {
                    break;
                }

                // Save items for each queryType
                token.ThrowIfCancellationRequested();
                fetchContext.SaveItems();

                // Delay between query types
                await Task.Delay(Random.Shared.Next(1000, 2000), token).ConfigureAwait(false);
            }
        }

        return new(!fetchContext.Status.AuthKeyTimeout, fetchContext.TargetArchive);
    }
}
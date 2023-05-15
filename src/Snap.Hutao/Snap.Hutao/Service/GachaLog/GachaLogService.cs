// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped, typeof(IGachaLogService))]
internal sealed partial class GachaLogService : IGachaLogService
{
    private readonly ScopedDbCurrent<GachaArchive, Message.GachaArchiveChangedMessage> dbCurrent;
    private readonly IGachaStatisticsSlimFactory gachaStatisticsSlimFactory;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly IUIGFExportService gachaLogExportService;
    private readonly IUIGFImportService gachaLogImportService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ILogger<GachaLogService> logger;
    private readonly GachaInfoClient gachaInfoClient;
    private readonly ITaskContext taskContext;

    private GachaLogServiceContext context;

    /// <inheritdoc/>
    public GachaArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<GachaArchive> ArchiveCollection
    {
        get => context.ArchiveCollection;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token)
    {
        if (context.IsInitialized)
        {
            return true;
        }

        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            Dictionary<AvatarId, Model.Metadata.Avatar.Avatar> idAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false);
            Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> idWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);

            Dictionary<string, Model.Metadata.Avatar.Avatar> nameAvatarMap = await metadataService.GetNameToAvatarMapAsync(token).ConfigureAwait(false);
            Dictionary<string, Model.Metadata.Weapon.Weapon> nameWeaponMap = await metadataService.GetNameToWeaponMapAsync(token).ConfigureAwait(false);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                GachaArchives.Initialize(appDbContext, out ObservableCollection<GachaArchive> collection);

                context = new(idAvatarMap, idWeaponMap, nameAvatarMap, nameWeaponMap, collection);
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<GachaStatistics> GetStatisticsAsync(GachaArchive? archive)
    {
        archive ??= CurrentArchive;

        // Return statistics
        if (archive != null)
        {
            using (ValueStopwatch.MeasureExecution(logger))
            {
                using (IServiceScope scope = serviceProvider.CreateScope())
                {
                    AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    IOrderedQueryable<GachaItem> items = appDbContext.GachaItems
                        .Where(i => i.ArchiveId == archive.InnerId)
                        .OrderBy(i => i.Id);

                    return await gachaStatisticsFactory.CreateAsync(items, context).ConfigureAwait(false);
                }
            }
        }
        else
        {
            throw Must.NeverHappen();
        }
    }

    /// <inheritdoc/>
    public async Task<List<GachaStatisticsSlim>> GetStatisticsSlimsAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            List<GachaStatisticsSlim> statistics = new();
            foreach (GachaArchive archive in appDbContext.GachaArchives)
            {
                IOrderedQueryable<GachaItem> items = appDbContext.GachaItems
                    .Where(i => i.ArchiveId == archive.InnerId)
                    .OrderBy(i => i.Id);

                GachaStatisticsSlim slim = await gachaStatisticsSlimFactory.CreateAsync(items, context).ConfigureAwait(false);
                slim.Uid = archive.Uid;
                statistics.Add(slim);
            }

            return statistics;
        }
    }

    /// <inheritdoc/>
    public Task<UIGF> ExportToUIGFAsync(GachaArchive archive)
    {
        return gachaLogExportService.ExportAsync(context, archive);
    }

    /// <inheritdoc/>
    public async Task ImportFromUIGFAsync(UIGF uigf)
    {
        CurrentArchive = await gachaLogImportService.ImportAsync(context, uigf).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategy strategy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        bool isLazy = strategy switch
        {
            RefreshStrategy.AggressiveMerge => false,
            RefreshStrategy.LazyMerge => true,
            _ => throw Must.NeverHappen(),
        };

        (bool authkeyValid, GachaArchive? result) = await FetchGachaLogsAsync(query, isLazy, progress, token).ConfigureAwait(false);

        if (result != null)
        {
            CurrentArchive = result;
        }

        return authkeyValid;
    }

    /// <inheritdoc/>
    public async Task RemoveArchiveAsync(GachaArchive archive)
    {
        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        context.ArchiveCollection.Remove(archive);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.GachaArchives
                .ExecuteDeleteWhereAsync(a => a.InnerId == archive.InnerId)
                .ConfigureAwait(false);
        }
    }

    private static Task RandomDelayAsync(CancellationToken token)
    {
        return Task.Delay(TimeSpan.FromSeconds(Random.Shared.NextDouble() + 1), token);
    }

    private async Task<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        GachaLogFetchContext fetchContext = new(serviceProvider, context, isLazy);

        foreach (GachaConfigType configType in GachaLog.QueryTypes)
        {
            fetchContext.ResetForProcessingType(configType, query);

            do
            {
                Response<GachaLogPage> response = await gachaInfoClient
                    .GetGachaLogPageAsync(fetchContext.QueryOptions, token)
                    .ConfigureAwait(false);

                if (response.TryGetData(out GachaLogPage? page, serviceProvider))
                {
                    List<GachaLogItem> items = page.List;
                    fetchContext.ResetForProcessingPage();

                    foreach (GachaLogItem item in items)
                    {
                        fetchContext.EnsureArchiveAndEndId(item);

                        if (fetchContext.ShouldAdd(item))
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
                }
                else
                {
                    fetchContext.Report(progress, true);
                    break;
                }

                await RandomDelayAsync(token).ConfigureAwait(false);
            }
            while (true);

            if (fetchContext.FetchStatus.AuthKeyTimeout)
            {
                break;
            }

            token.ThrowIfCancellationRequested();
            fetchContext.SaveItems();
            await RandomDelayAsync(token).ConfigureAwait(false);
        }

        return new(!fetchContext.FetchStatus.AuthKeyTimeout, fetchContext.TargetArchive);
    }
}
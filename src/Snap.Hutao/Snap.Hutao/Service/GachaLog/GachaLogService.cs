// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Model.Entity;
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
    private readonly IMetadataService metadataService;
    private readonly ILogger<GachaLogService> logger;
    private readonly GachaInfoClient gachaInfoClient;
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly ITaskContext taskContext;

    private GachaLogServiceMetadataContext context;
    private ObservableCollection<GachaArchive>? archiveCollection;

    /// <inheritdoc/>
    public GachaArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public ObservableCollection<GachaArchive>? ArchiveCollection
    {
        get => archiveCollection;
        private set => archiveCollection = value;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
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
            context = new(idAvatarMap, idWeaponMap, nameAvatarMap, nameWeaponMap);

            ArchiveCollection = gachaLogDbService.GetGachaArchiveCollection();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<GachaStatistics> GetStatisticsAsync(GachaArchive? archive)
    {
        archive ??= CurrentArchive;
        archive ??= ArchiveCollection?.FirstOrDefault();
        ArgumentNullException.ThrowIfNull(archive);

        // Return statistics
        using (ValueStopwatch.MeasureExecution(logger))
        {
            List<GachaItem> items = await gachaLogDbService.GetGachaItemListByArchiveIdAsync(archive.InnerId).ConfigureAwait(false);
            return await gachaStatisticsFactory.CreateAsync(items, context).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask<List<GachaStatisticsSlim>> GetStatisticsSlimListAsync(CancellationToken token = default)
    {
        await InitializeAsync(token).ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(ArchiveCollection);

        List<GachaStatisticsSlim> statistics = new();
        foreach (GachaArchive archive in ArchiveCollection)
        {
            List<GachaItem> items = await gachaLogDbService.GetGachaItemListByArchiveIdAsync(archive.InnerId).ConfigureAwait(false);
            GachaStatisticsSlim slim = await gachaStatisticsSlimFactory.CreateAsync(context, items, archive.Uid).ConfigureAwait(false);
            statistics.Add(slim);
        }

        return statistics;
    }

    /// <inheritdoc/>
    public ValueTask<UIGF> ExportToUIGFAsync(GachaArchive archive)
    {
        return gachaLogExportService.ExportAsync(context, archive);
    }

    /// <inheritdoc/>
    public async ValueTask ImportFromUIGFAsync(UIGF uigf)
    {
        ArgumentNullException.ThrowIfNull(ArchiveCollection);
        CurrentArchive = await gachaLogImportService.ImportAsync(context, uigf, ArchiveCollection).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategy strategy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        bool isLazy = strategy switch
        {
            RefreshStrategy.AggressiveMerge => false,
            RefreshStrategy.LazyMerge => true,
            _ => throw Must.NeverHappen(),
        };

        (bool authkeyValid, GachaArchive? result) = await FetchGachaLogsAsync(query, isLazy, progress, token).ConfigureAwait(false);

        if (result is not null)
        {
            CurrentArchive = result;
        }

        return authkeyValid;
    }

    /// <inheritdoc/>
    public async ValueTask RemoveArchiveAsync(GachaArchive archive)
    {
        ArgumentNullException.ThrowIfNull(archiveCollection);

        // Sync database
        await taskContext.SwitchToBackgroundAsync();
        await gachaLogDbService.RemoveGachaArchiveByIdAsync(archive.InnerId).ConfigureAwait(false);

        // Sync cache
        await taskContext.SwitchToMainThreadAsync();
        archiveCollection.Remove(archive);
    }

    public async ValueTask<GachaArchive> EnsureArchiveInCollectionAsync(Guid archiveId, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(ArchiveCollection);

        if (ArchiveCollection.SingleOrDefault(a => a.InnerId == archiveId) is { } archive)
        {
            return archive;
        }
        else
        {
            // sync cache
            GachaArchive? newArchive = await gachaLogDbService.GetGachaArchiveByIdAsync(archiveId, token).ConfigureAwait(false);
            ArgumentNullException.ThrowIfNull(newArchive);

            await taskContext.SwitchToMainThreadAsync();
            ArchiveCollection.Add(newArchive);
            return newArchive;
        }
    }

    private async ValueTask<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchStatus> progress, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(ArchiveCollection);
        GachaLogFetchContext fetchContext = new(gachaLogDbService, taskContext, context, isLazy);

        foreach (GachaConfigType configType in GachaLog.QueryTypes)
        {
            fetchContext.ResetForProcessingType(configType, query);

            do
            {
                Response<GachaLogPage> response = await gachaInfoClient
                    .GetGachaLogPageAsync(fetchContext.QueryOptions, token)
                    .ConfigureAwait(false);

                if (response.TryGetData(out GachaLogPage? page))
                {
                    List<GachaLogItem> items = page.List;
                    fetchContext.ResetForProcessingPage();

                    foreach (GachaLogItem item in items)
                    {
                        fetchContext.EnsureArchiveAndEndId(item, ArchiveCollection, gachaLogDbService);

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
                }
                else
                {
                    fetchContext.Report(progress, true);
                    break;
                }

                await Delay.Random(1000, 2000).ConfigureAwait(false);
            }
            while (true);

            if (fetchContext.FetchStatus.AuthKeyTimeout)
            {
                break;
            }

            // save items for each queryType
            token.ThrowIfCancellationRequested();
            fetchContext.SaveItems();
            await Delay.Random(1000, 2000).ConfigureAwait(false);
        }

        return new(!fetchContext.FetchStatus.AuthKeyTimeout, fetchContext.TargetArchive);
    }
}
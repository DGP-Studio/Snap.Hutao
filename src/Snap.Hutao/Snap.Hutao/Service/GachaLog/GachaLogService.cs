// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务
/// </summary>
[HighQuality]
[Injection(InjectAs.Scoped, typeof(IGachaLogService))]
internal sealed class GachaLogService : IGachaLogService
{
    private readonly AppDbContext appDbContext;
    private readonly IGachaLogExportService gachaLogExportService;
    private readonly IGachaLogImportService gachaLogImportService;

    private readonly IEnumerable<IGachaLogQueryProvider> urlProviders;
    private readonly GachaInfoClient gachaInfoClient;
    private readonly IMetadataService metadataService;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly ILogger<GachaLogService> logger;
    private readonly DbCurrent<GachaArchive, Message.GachaArchiveChangedMessage> dbCurrent;

    private GachaLogServiceContext context;

    /// <summary>
    /// 构造一个新的祈愿记录服务
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="urlProviders">Url提供器集合</param>
    /// <param name="gachaInfoClient">祈愿记录客户端</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="gachaStatisticsFactory">祈愿统计工厂</param>
    /// <param name="logger">日志器</param>
    /// <param name="messenger">消息器</param>
    public GachaLogService(
        IServiceProvider serviceProvider,
        AppDbContext appDbContext,
        IEnumerable<IGachaLogQueryProvider> urlProviders,
        GachaInfoClient gachaInfoClient,
        IMetadataService metadataService,
        IGachaStatisticsFactory gachaStatisticsFactory,
        ILogger<GachaLogService> logger,
        IMessenger messenger)
    {
        gachaLogExportService = serviceProvider.GetRequiredService<IGachaLogExportService>();
        gachaLogImportService = serviceProvider.GetRequiredService<IGachaLogImportService>();

        this.appDbContext = appDbContext;
        this.urlProviders = urlProviders;
        this.gachaInfoClient = gachaInfoClient;
        this.metadataService = metadataService;
        this.logger = logger;
        this.gachaStatisticsFactory = gachaStatisticsFactory;

        dbCurrent = new(appDbContext.GachaArchives, messenger);
    }

    /// <inheritdoc/>
    public GachaArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
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

            GachaArchives.Initialize(appDbContext, out ObservableCollection<GachaArchive> collection);
            context = new(idAvatarMap, idWeaponMap, nameAvatarMap, nameWeaponMap, collection);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public ObservableCollection<GachaArchive> GetArchiveCollection()
    {
        return context.ArchiveCollection;
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
                IQueryable<GachaItem> items = appDbContext.GachaItems.Where(i => i.ArchiveId == archive.InnerId);
                return await gachaStatisticsFactory.CreateAsync(items).ConfigureAwait(false);
            }
        }
        else
        {
            throw Must.NeverHappen();
        }
    }

    /// <inheritdoc/>
    public Task<UIGF> ExportToUIGFAsync(GachaArchive archive)
    {
        return gachaLogExportService.ExportToUIGFAsync(context, archive);
    }

    /// <inheritdoc/>
    public async Task ImportFromUIGFAsync(List<UIGFItem> list, string uid)
    {
        CurrentArchive = await gachaLogImportService.ImportFromUIGFAsync(context, list, uid).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategy strategy, IProgress<GachaLogFetchState> progress, CancellationToken token)
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
        await ThreadHelper.SwitchToMainThreadAsync();
        context.ArchiveCollection.Remove(archive);

        // Sync database
        await ThreadHelper.SwitchToBackgroundAsync();
        await appDbContext.GachaArchives
            .Where(a => a.InnerId == archive.InnerId)
            .ExecuteDeleteAsync()
            .ConfigureAwait(false);
    }

    private static Task RandomDelayAsync(CancellationToken token)
    {
        return Task.Delay(TimeSpan.FromSeconds(Random.Shared.NextDouble() + 1), token);
    }

    private async Task<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogQuery query, bool isLazy, IProgress<GachaLogFetchState> progress, CancellationToken token)
    {
        GachaArchive? archive = null;
        GachaLogFetchState state = new();

        foreach (GachaConfigType configType in GachaLog.QueryTypes)
        {
            // 每个卡池类型重置
            long? dbEndId = null;
            state.ConfigType = configType;
            GachaLogQueryOptions options = new(query, configType);
            List<GachaItem> itemsToAdd = new();

            do
            {
                Response<GachaLogPage> response = await gachaInfoClient.GetGachaLogPageAsync(options, token).ConfigureAwait(false);

                if (response.IsOk())
                {
                    GachaLogPage page = response.Data;

                    state.Items.Clear();
                    List<GachaLogItem> items = page.List;
                    bool currentTypeAddingCompleted = false;

                    foreach (GachaLogItem item in items)
                    {
                        GachaArchive.SkipOrInit(ref archive, item.Uid, appDbContext.GachaArchives, context.ArchiveCollection);
                        dbEndId ??= archive.GetEndId(configType, appDbContext.GachaItems);

                        if ((!isLazy) || item.Id > dbEndId)
                        {
                            itemsToAdd.Add(GachaItem.Create(archive.InnerId, item, context.GetItemId(item)));
                            state.Items.Add(context.GetItemByNameAndType(item.Name, item.ItemType));
                            options.EndId = item.Id;
                        }
                        else
                        {
                            currentTypeAddingCompleted = true;
                            break;
                        }
                    }

                    progress.Report(state);

                    if (currentTypeAddingCompleted || items.Count < GachaLogQueryOptions.Size)
                    {
                        // exit current type fetch loop
                        break;
                    }
                }
                else
                {
                    state.AuthKeyTimeout = true;
                    progress.Report(state);
                    break;
                }

                await RandomDelayAsync(token).ConfigureAwait(false);
            }
            while (true);

            if (state.AuthKeyTimeout)
            {
                break;
            }

            token.ThrowIfCancellationRequested();
            archive?.SaveItems(itemsToAdd, isLazy, options.EndId, appDbContext.GachaItems);
            await RandomDelayAsync(token).ConfigureAwait(false);
        }

        return new(!state.AuthKeyTimeout, archive);
    }
}
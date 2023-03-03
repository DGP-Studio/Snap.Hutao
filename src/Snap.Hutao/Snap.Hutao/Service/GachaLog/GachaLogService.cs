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
    /// <summary>
    /// 祈愿记录查询的类型
    /// </summary>
    private static readonly ImmutableList<GachaConfigType> QueryTypes = new List<GachaConfigType>
    {
        GachaConfigType.NoviceWish,
        GachaConfigType.StandardWish,
        GachaConfigType.AvatarEventWish,
        GachaConfigType.WeaponEventWish,
    }.ToImmutableList();

    private readonly AppDbContext appDbContext;
    private readonly IEnumerable<IGachaLogQueryProvider> urlProviders;
    private readonly GachaInfoClient gachaInfoClient;
    private readonly IMetadataService metadataService;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly ILogger<GachaLogService> logger;
    private readonly DbCurrent<GachaArchive, Message.GachaArchiveChangedMessage> dbCurrent;

    private readonly Dictionary<string, Item> itemBaseCache = new();

    private Dictionary<string, Model.Metadata.Avatar.Avatar>? nameAvatarMap;
    private Dictionary<string, Model.Metadata.Weapon.Weapon>? nameWeaponMap;

    private Dictionary<AvatarId, Model.Metadata.Avatar.Avatar>? idAvatarMap;
    private Dictionary<WeaponId, Model.Metadata.Weapon.Weapon>? idWeaponMap;

    private ObservableCollection<GachaArchive>? archiveCollection;

    /// <summary>
    /// 构造一个新的祈愿记录服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="urlProviders">Url提供器集合</param>
    /// <param name="gachaInfoClient">祈愿记录客户端</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="gachaStatisticsFactory">祈愿统计工厂</param>
    /// <param name="logger">日志器</param>
    /// <param name="messenger">消息器</param>
    public GachaLogService(
        AppDbContext appDbContext,
        IEnumerable<IGachaLogQueryProvider> urlProviders,
        GachaInfoClient gachaInfoClient,
        IMetadataService metadataService,
        IGachaStatisticsFactory gachaStatisticsFactory,
        ILogger<GachaLogService> logger,
        IMessenger messenger)
    {
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
    public Task<UIGF> ExportToUIGFAsync(GachaArchive archive)
    {
        List<UIGFItem> list = appDbContext.GachaItems
            .Where(i => i.ArchiveId == archive.InnerId)
            .AsEnumerable()
            .Select(i => i.ToUIGFItem(GetNameQualityByItemId(i.ItemId)))
            .ToList();

        UIGF uigf = new()
        {
            Info = UIGFInfo.Create(archive.Uid),
            List = list,
        };

        return Task.FromResult(uigf);
    }

    /// <inheritdoc/>
    public async Task<ObservableCollection<GachaArchive>> GetArchiveCollectionAsync()
    {
        await ThreadHelper.SwitchToMainThreadAsync();
        try
        {
            archiveCollection ??= appDbContext.GachaArchives.AsNoTracking().ToObservableCollection();
        }
        catch (SqliteException ex)
        {
            ThrowHelper.UserdataCorrupted(string.Format(SH.ServiceGachaLogArchiveCollectionUserdataCorruptedMessage, ex.Message), ex);
        }

        return archiveCollection;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token)
    {
        if (await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            nameAvatarMap = await metadataService.GetNameToAvatarMapAsync(token).ConfigureAwait(false);
            nameWeaponMap = await metadataService.GetNameToWeaponMapAsync(token).ConfigureAwait(false);

            idAvatarMap = await metadataService.GetIdToAvatarMapAsync(token).ConfigureAwait(false);
            idWeaponMap = await metadataService.GetIdToWeaponMapAsync(token).ConfigureAwait(false);

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<GachaStatistics> GetStatisticsAsync(GachaArchive? archive = null)
    {
        archive ??= CurrentArchive;

        // Return statistics
        if (archive != null)
        {
            ValueStopwatch stopwatch = ValueStopwatch.StartNew();
            IQueryable<GachaItem> items = appDbContext.GachaItems
                .Where(i => i.ArchiveId == archive.InnerId);

            GachaStatistics statistics = await gachaStatisticsFactory.CreateAsync(items).ConfigureAwait(false);

            logger.LogInformation("GachaStatistic Generation toke {time} ms.", stopwatch.GetElapsedTime().TotalMilliseconds);
            return statistics;
        }
        else
        {
            throw Must.NeverHappen();
        }
    }

    /// <inheritdoc/>
    public IGachaLogQueryProvider? GetGachaLogQueryProvider(RefreshOption option)
    {
        return option switch
        {
            RefreshOption.WebCache => urlProviders.Single(p => p.Name == nameof(GachaLogQueryWebCacheProvider)),
            RefreshOption.Stoken => urlProviders.Single(p => p.Name == nameof(GachaLogQueryStokenProvider)),
            RefreshOption.ManualInput => urlProviders.Single(p => p.Name == nameof(GachaLogQueryManualInputProvider)),
            _ => null,
        };
    }

    /// <inheritdoc/>
    public async Task ImportFromUIGFAsync(List<UIGFItem> list, string uid)
    {
        await ThreadHelper.SwitchToBackgroundAsync();

        GachaArchive? archive = null;
        SkipOrInitArchive(ref archive, uid);
        Guid archiveId = archive.InnerId;

        long trimId = appDbContext.GachaItems
            .Where(i => i.ArchiveId == archiveId)
            .OrderBy(i => i.Id)
            .FirstOrDefault()?.Id ?? long.MaxValue;

        logger.LogInformation("Last Id to trim with [{id}]", trimId);

        IEnumerable<GachaItem> toAdd = list
            .OrderByDescending(i => i.Id)
            .Where(i => i.Id < trimId)
            .Select(i => GachaItem.Create(archiveId, i, GetItemId(i)));

        await appDbContext.GachaItems.AddRangeAndSaveAsync(toAdd).ConfigureAwait(false);
        CurrentArchive = archive;
    }

    /// <inheritdoc/>
    public async Task<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategy strategy, IProgress<FetchState> progress, CancellationToken token)
    {
        bool isLazy = strategy switch
        {
            RefreshStrategy.AggressiveMerge => false,
            RefreshStrategy.LazyMerge => true,
            _ => throw Must.NeverHappen(),
        };

        (bool authkeyValid, GachaArchive? result) = await FetchGachaLogsAsync(query, isLazy, progress, token).ConfigureAwait(false);
        CurrentArchive = result ?? CurrentArchive;
        return authkeyValid;
    }

    /// <inheritdoc/>
    public async Task RemoveArchiveAsync(GachaArchive archive)
    {
        // Sync cache
        await ThreadHelper.SwitchToMainThreadAsync();
        archiveCollection!.Remove(archive);

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

    private async Task<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(GachaLogQuery query, bool isLazy, IProgress<FetchState> progress, CancellationToken token)
    {
        GachaArchive? archive = null;
        FetchState state = new();

        foreach (GachaConfigType configType in QueryTypes)
        {
            state.ConfigType = configType;
            long? dbEndId = null;
            GachaLogQueryOptions configration = new(query, configType);
            List<GachaItem> itemsToAdd = new();

            do
            {
                Response<GachaLogPage> response = await gachaInfoClient.GetGachaLogPageAsync(configration, token).ConfigureAwait(false);

                if (response.IsOk())
                {
                    GachaLogPage page = response.Data;

                    state.Items.Clear();
                    List<GachaLogItem> items = page.List;
                    bool currentTypeAddingCompleted = false;

                    foreach (GachaLogItem item in items)
                    {
                        SkipOrInitArchive(ref archive, item.Uid);
                        dbEndId ??= GetEndId(archive, configType);

                        if ((!isLazy) || item.Id > dbEndId)
                        {
                            itemsToAdd.Add(GachaItem.Create(archive.InnerId, item, GetItemId(item)));
                            state.Items.Add(GetItemBaseByName(item.Name, item.ItemType));
                            configration.EndId = item.Id;
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
            SaveGachaItems(itemsToAdd, isLazy, archive, configration.EndId);
            await RandomDelayAsync(token).ConfigureAwait(false);
        }

        return new(!state.AuthKeyTimeout, archive);
    }

    private void SkipOrInitArchive([NotNull] ref GachaArchive? archive, string uid)
    {
        if (archive == null)
        {
            archive = appDbContext.GachaArchives.SingleOrDefault(a => a.Uid == uid);

            if (archive == null)
            {
                GachaArchive created = GachaArchive.Create(uid);
                appDbContext.GachaArchives.AddAndSave(created);

                // System.InvalidOperationException: Sequence contains no elements
                // ? how this happen here?
                archive = appDbContext.GachaArchives.Single(a => a.Uid == uid);
                GachaArchive temp = archive;
                ThreadHelper.InvokeOnMainThread(() => archiveCollection!.Add(temp));
            }
        }
    }

    private long GetEndId(GachaArchive? archive, GachaConfigType configType)
    {
        GachaItem? item = null;

        if (archive != null)
        {
            try
            {
                // TODO: replace with MaxBy
                // https://github.com/dotnet/efcore/issues/25566
                // .MaxBy(i => i.Id);
                item = appDbContext.GachaItems
                    .Where(i => i.ArchiveId == archive.InnerId)
                    .Where(i => i.QueryType == configType)
                    .OrderByDescending(i => i.Id)
                    .FirstOrDefault();
            }
            catch (SqliteException ex)
            {
                ThrowHelper.UserdataCorrupted(SH.ServiceGachaLogEndIdUserdataCorruptedMessage, ex);
            }
        }

        return item?.Id ?? 0L;
    }

    private int GetItemId(GachaLogItem item)
    {
        return item.ItemType switch
        {
            "角色" => nameAvatarMap!.GetValueOrDefault(item.Name)?.Id ?? 0,
            "武器" => nameWeaponMap!.GetValueOrDefault(item.Name)?.Id ?? 0,
            _ => 0,
        };
    }

    private Item GetItemBaseByName(string name, string type)
    {
        if (!itemBaseCache.TryGetValue(name, out Item? result))
        {
            result = type switch
            {
                "角色" => nameAvatarMap![name].ToItemBase(),
                "武器" => nameWeaponMap![name].ToItemBase(),
                _ => throw Must.NeverHappen(),
            };

            itemBaseCache[name] = result;
        }

        return result;
    }

    private INameQuality GetNameQualityByItemId(int id)
    {
        int place = id.Place();
        return place switch
        {
            8 => idAvatarMap![id],
            5 => idWeaponMap![id],
            _ => throw Must.NeverHappen($"Id places: {place}"),
        };
    }

    private void SaveGachaItems(List<GachaItem> itemsToAdd, bool isLazy, GachaArchive? archive, long endId)
    {
        if (itemsToAdd.Count > 0)
        {
            // 全量刷新
            if ((!isLazy) && archive != null)
            {
                appDbContext.GachaItems
                    .Where(i => i.ArchiveId == archive.InnerId)
                    .Where(i => i.Id >= endId)
                    .ExecuteDelete();
            }

            appDbContext.GachaItems.AddRangeAndSave(itemsToAdd);
        }
    }
}
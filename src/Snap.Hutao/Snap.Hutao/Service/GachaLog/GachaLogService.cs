// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Messaging;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Binding.Gacha;
using Snap.Hutao.Model.Binding.Gacha.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Model.Metadata.Abstraction;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.GachaLog.Factory;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using Snap.Hutao.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务
/// </summary>
[Injection(InjectAs.Transient, typeof(IGachaLogService))]
internal class GachaLogService : IGachaLogService, ISupportAsyncInitialization
{
    /// <summary>
    /// 祈愿记录查询的类型
    /// </summary>
    private static readonly ImmutableList<GachaConfigType> QueryTypes = ImmutableList.Create(new GachaConfigType[]
    {
        GachaConfigType.NoviceWish,
        GachaConfigType.PermanentWish,
        GachaConfigType.AvatarEventWish,
        GachaConfigType.WeaponEventWish,
    });

    private readonly AppDbContext appDbContext;
    private readonly IEnumerable<IGachaLogUrlProvider> urlProviders;
    private readonly GachaInfoClient gachaInfoClient;
    private readonly IMetadataService metadataService;
    private readonly IInfoBarService infoBarService;
    private readonly IGachaStatisticsFactory gachaStatisticsFactory;
    private readonly DbCurrent<GachaArchive, Message.GachaArchiveChangedMessage> dbCurrent;

    private readonly Dictionary<string, ItemBase> itemBaseCache = new();

    private Dictionary<string, Model.Metadata.Avatar.Avatar>? nameAvatarMap;
    private Dictionary<string, Model.Metadata.Weapon.Weapon>? nameWeaponMap;

    private Dictionary<int, Model.Metadata.Avatar.Avatar>? idAvatarMap;
    private Dictionary<int, Model.Metadata.Weapon.Weapon>? idWeaponMap;
    private ObservableCollection<GachaArchive>? archiveCollection;

    /// <summary>
    /// 构造一个新的祈愿记录服务
    /// </summary>
    /// <param name="appDbContext">数据库上下文</param>
    /// <param name="urlProviders">Url提供器集合</param>
    /// <param name="gachaInfoClient">祈愿记录客户端</param>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="gachaStatisticsFactory">祈愿统计工厂</param>
    /// <param name="messenger">消息器</param>
    public GachaLogService(
        AppDbContext appDbContext,
        IEnumerable<IGachaLogUrlProvider> urlProviders,
        GachaInfoClient gachaInfoClient,
        IMetadataService metadataService,
        IInfoBarService infoBarService,
        IGachaStatisticsFactory gachaStatisticsFactory,
        IMessenger messenger)
    {
        this.appDbContext = appDbContext;
        this.urlProviders = urlProviders;
        this.gachaInfoClient = gachaInfoClient;
        this.metadataService = metadataService;
        this.infoBarService = infoBarService;
        this.gachaStatisticsFactory = gachaStatisticsFactory;

        dbCurrent = new(appDbContext, appDbContext.GachaArchives, messenger);
    }

    /// <inheritdoc/>
    public GachaArchive? CurrentArchive
    {
        get => dbCurrent.Current;
        set => dbCurrent.Current = value;
    }

    /// <inheritdoc/>
    public bool IsInitialized { get; set; }

    /// <inheritdoc/>
    public Task<UIGF> ExportToUIGFAsync(GachaArchive archive)
    {
        Verify.Operation(IsInitialized, "祈愿记录服务未能正常初始化");

        var list = appDbContext.GachaItems
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
    public ObservableCollection<GachaArchive> GetArchiveCollection()
    {
        return archiveCollection ??= new(appDbContext.GachaArchives.ToList());
    }

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync(CancellationToken token = default)
    {
        if (await metadataService.InitializeAsync(token).ConfigureAwait(false))
        {
            nameAvatarMap = await metadataService.GetNameToAvatarMapAsync(token).ConfigureAwait(false);
            nameWeaponMap = await metadataService.GetNameToWeaponMapAsync(token).ConfigureAwait(false);

            idAvatarMap = await metadataService.GetIdToAvatarMapAsync().ConfigureAwait(false);
            idWeaponMap = await metadataService.GetIdToWeaponMapAsync().ConfigureAwait(false);

            IsInitialized = true;
        }
        else
        {
            IsInitialized = false;
        }

        return IsInitialized;
    }

    /// <inheritdoc/>
    public Task<GachaStatistics> GetStatisticsAsync(GachaArchive? archive = null)
    {
        archive ??= CurrentArchive;

        // Return statistics
        if (archive != null)
        {
            IQueryable<GachaItem> items = appDbContext.GachaItems
                .Where(i => i.ArchiveId == archive.InnerId);

            return gachaStatisticsFactory.CreateAsync(items);
        }
        else
        {
            return Must.Fault<GachaStatistics>("没有选中的存档");
        }
    }

    /// <inheritdoc/>
    public IGachaLogUrlProvider? GetGachaLogUrlProvider(RefreshOption option)
    {
        return option switch
        {
            RefreshOption.WebCache => urlProviders.Single(p => p.Name == nameof(GachaLogUrlWebCacheProvider)),
            RefreshOption.Stoken => urlProviders.Single(p => p.Name == nameof(GachaLogUrlStokenProvider)),
            RefreshOption.ManualInput => urlProviders.Single(p => p.Name == nameof(GachaLogUrlManualInputProvider)),
            _ => null,
        };
    }

    /// <inheritdoc/>
    public Task ImportFromUIGFAsync(List<UIGFItem> list, string uid)
    {
        return Task.Run(() => ImportFromUIGF(list, uid));
    }

    /// <inheritdoc/>
    public async Task<bool> RefreshGachaLogAsync(string query, RefreshStrategy strategy, IProgress<FetchState> progress, CancellationToken token)
    {
        Verify.Operation(IsInitialized, "祈愿记录服务未能正常初始化");

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
    public Task RemoveArchiveAsync(GachaArchive archive)
    {
        Must.NotNull(archiveCollection!);

        // Sync cache
        archiveCollection.Remove(archive);

        // Sync database
        appDbContext.GachaArchives.Remove(archive);
        return appDbContext.SaveChangesAsync();
    }

    private static Task RandomDelayAsync(CancellationToken token)
    {
        return Task.Delay(TimeSpan.FromSeconds(Random.Shared.NextDouble() + 1), token);
    }

    private void ImportFromUIGF(List<UIGFItem> list, string uid)
    {
        GachaArchive? archive = null;
        SkipOrInitArchive(ref archive, uid);
        Guid archiveId = Must.NotNull(archive!).InnerId;

        long trimId = appDbContext.GachaItems
            .Where(i => i.ArchiveId == archiveId)
            .OrderBy(i => i.Id)
            .Take(1)
            .FirstOrDefault()?.Id ?? long.MaxValue;

        IEnumerable<GachaItem> toAdd = list
            .OrderByDescending(i => i.Id)
            .Where(i => i.Id < trimId)
            .Select(i => GachaItem.Create(archiveId, i, GetItemId(i)));

        appDbContext.GachaItems.AddRange(toAdd);
        appDbContext.SaveChanges();

        CurrentArchive = archive;
    }

    private async Task<ValueResult<bool, GachaArchive?>> FetchGachaLogsAsync(string query, bool isLazy, IProgress<FetchState> progress, CancellationToken token)
    {
        GachaArchive? archive = null;
        FetchState state = new();

        foreach (GachaConfigType configType in QueryTypes)
        {
            state.ConfigType = configType;
            long? dbEndId = null;
            GachaLogConfigration configration = new(query, configType);
            List<GachaItem> itemsToAdd = new();

            do
            {
                Response<GachaLogPage>? response = await gachaInfoClient.GetGachaLogPageAsync(configration, token).ConfigureAwait(false);

                if (response?.Data is GachaLogPage page)
                {
                    state.Items.Clear();
                    List<GachaLogItem> items = page.List;
                    bool completedCurrentTypeAdding = false;

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
                            completedCurrentTypeAdding = true;
                            break;
                        }
                    }

                    progress.Report(state);

                    if (completedCurrentTypeAdding || items.Count < GachaLogConfigration.Size)
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
                appDbContext.GachaArchives.Add(created);
                appDbContext.SaveChanges();

                archive = appDbContext.GachaArchives.Single(a => a.Uid == uid);
                GachaArchive temp = archive;
                Program.DispatcherQueue!.TryEnqueue(() => archiveCollection!.Add(temp));
            }
        }
    }

    private long GetEndId(GachaArchive? archive, GachaConfigType configType)
    {
        GachaItem? item = null;

        if (archive != null)
        {
            item = appDbContext.GachaItems
                .Where(i => i.ArchiveId == archive.InnerId)
                .Where(i => i.QueryType == configType)
                .OrderByDescending(i => i.Id)
                .Take(1)
                .FirstOrDefault();

            // MaxBy should be supported by .NET 7
            // .MaxBy(i => i.Id);
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

    private ItemBase GetItemBaseByName(string name, string type)
    {
        if (!itemBaseCache.TryGetValue(name, out ItemBase? result))
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
        return id.Place() switch
        {
            8 => idAvatarMap![id],
            5 => idWeaponMap![id],
            _ => throw Must.NeverHappen(),
        };
    }

    private void SaveGachaItems(List<GachaItem> itemsToAdd, bool isLazy, GachaArchive? archive, long endId)
    {
        if (itemsToAdd.Count > 0)
        {
            if ((!isLazy) && archive != null)
            {
                IQueryable<GachaItem> toRemove = appDbContext.GachaItems
                    .Where(i => i.ArchiveId == archive.InnerId)
                    .Where(i => i.Id >= endId);

                appDbContext.GachaItems.RemoveRange(toRemove);
                appDbContext.SaveChanges();
            }

            appDbContext.GachaItems.AddRange(itemsToAdd);
            appDbContext.SaveChanges();
        }
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

namespace Snap.Hutao.Service.GachaLog;

internal struct GachaLogFetchContext
{
    public GachaArchive? TargetArchive;
    public GachaLogFetchStatus FetchStatus = default!;
    public long? DbEndId;
    public GachaLogTypedQueryOptions TypedQueryOptions;
    public List<GachaItem> ItemsToAdd = default!;
    public bool CurrentTypeAddingCompleted;
    public GachaType CurrentType;

    private readonly GachaLogServiceMetadataContext serviceContext;
    private readonly IGachaLogDbService gachaLogDbService;
    private readonly ITaskContext taskContext;
    private readonly bool isLazy;

    public GachaLogFetchContext(IGachaLogDbService gachaLogDbService, ITaskContext taskContext, GachaLogServiceMetadataContext serviceContext, bool isLazy)
    {
        this.gachaLogDbService = gachaLogDbService;
        this.taskContext = taskContext;
        this.serviceContext = serviceContext;
        this.isLazy = isLazy;
    }

    public void ResetForProcessingType(GachaType configType, in GachaLogQuery query)
    {
        DbEndId = null;
        CurrentType = configType;
        ItemsToAdd = [];
        FetchStatus = new(configType);
        TypedQueryOptions = new(query, configType);
    }

    public void ResetForProcessingPage()
    {
        FetchStatus = new(CurrentType);
        CurrentTypeAddingCompleted = false;
    }

    public void EnsureArchiveAndEndId(GachaLogItem item, AdvancedDbCollectionView<GachaArchive> archives, IGachaLogDbService gachaLogDbService)
    {
        if (TargetArchive is null)
        {
            GachaArchiveOperation.GetOrAdd(gachaLogDbService, taskContext, item.Uid, archives, out TargetArchive);
        }

        DbEndId ??= gachaLogDbService.GetNewestGachaItemIdByArchiveIdAndQueryType(TargetArchive.InnerId, CurrentType);
    }

    public readonly bool ShouldAddItem(GachaLogItem item)
    {
        return !isLazy || item.Id > DbEndId;
    }

    public readonly bool ItemsHaveReachEnd(List<GachaLogItem> items)
    {
        return CurrentTypeAddingCompleted || items.Count < GachaLogTypedQueryOptions.Size;
    }

    public void AddItem(GachaLogItem item)
    {
        ArgumentNullException.ThrowIfNull(TargetArchive);
        ItemsToAdd.Add(GachaItem.From(TargetArchive.InnerId, item, serviceContext.GetItemId(item)));
        FetchStatus.Items.Add(serviceContext.GetItemByNameAndType(item.Name, item.ItemType));
        TypedQueryOptions.EndId = item.Id;
    }

    public readonly void SaveItems()
    {
        // While no item is fetched, archive can be null.
        if (TargetArchive is not null)
        {
            if (ItemsToAdd.Count <= 0)
            {
                return;
            }

            // 全量刷新
            if (!isLazy)
            {
                gachaLogDbService.RemoveGachaItemRangeByArchiveIdAndQueryTypeNewerThanEndId(TargetArchive.InnerId, TypedQueryOptions.Type, TypedQueryOptions.EndId);
            }

            gachaLogDbService.AddGachaItemRange(ItemsToAdd);
        }
    }

    public void CompleteAdding()
    {
        CurrentTypeAddingCompleted = true;
    }

    public readonly void Report(IProgress<GachaLogFetchStatus> progress, bool isAuthKeyTimeout = false)
    {
        FetchStatus.AuthKeyTimeout = isAuthKeyTimeout;
        progress.Report(FetchStatus);
    }
}
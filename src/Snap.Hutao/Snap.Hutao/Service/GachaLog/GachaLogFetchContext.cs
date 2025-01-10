// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Snap.Hutao.Service.GachaLog;

internal struct GachaLogFetchContext
{
    public GachaArchive? TargetArchive;
    public GachaLogFetchStatus FetchStatus = default!;
    public long? DbEndId;
    public GachaLogTypedQueryOptions TypedQueryOptions;
    public List<GachaItem> ItemsToAdd = [];
    public bool CurrentTypeAddingCompleted;
    public GachaType CurrentType;

    private readonly GachaLogServiceMetadataContext serviceContext;
    private readonly IGachaLogRepository repository;
    private readonly bool isLazy;

    public GachaLogFetchContext(IGachaLogRepository repository, GachaLogServiceMetadataContext serviceContext, bool isLazy)
    {
        this.repository = repository;
        this.serviceContext = serviceContext;
        this.isLazy = isLazy;
    }

    public void ResetType(GachaType configType, in GachaLogQuery query)
    {
        DbEndId = null;
        CurrentType = configType;
        ItemsToAdd.Clear();
        FetchStatus = new(configType);
        TypedQueryOptions = new(query, configType);
        CurrentTypeAddingCompleted = false;
    }

    public void ResetCurrentPage()
    {
        FetchStatus = new(CurrentType);
    }

    public void EnsureArchiveAndEndId(GachaLogItem item, IAdvancedDbCollectionView<GachaArchive> archives, IGachaLogRepository repository)
    {
        TargetArchive ??= GachaArchiveOperation.GetOrAdd(repository, item.Uid, archives);
        DbEndId ??= repository.GetNewestGachaItemIdByArchiveIdAndQueryType(TargetArchive.InnerId, CurrentType);
    }

    public readonly bool ShouldAddItem(GachaLogItem item)
    {
        // For non-lazy mode, all items should be added
        return !isLazy || item.Id > DbEndId;
    }

    public readonly bool HasReachCurrentTypeEnd(ImmutableArray<GachaLogItem> items)
    {
        return CurrentTypeAddingCompleted || items.Length < GachaLogTypedQueryOptions.Size;
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
        if (TargetArchive is null)
        {
            Debug.Assert(ItemsToAdd.Count is 0);
            return;
        }

        // Current type has no item fetched
        if (ItemsToAdd.Count <= 0)
        {
            return;
        }

        // Aggressive mode: Remove all items of the same type and newer than end id
        if (!isLazy)
        {
            repository.RemoveGachaItemRangeByArchiveIdAndQueryTypeNewerThanEndId(TargetArchive.InnerId, TypedQueryOptions.Type, TypedQueryOptions.EndId);
        }

        repository.AddGachaItemRange(ItemsToAdd);
    }

    public void CompleteCurrentTypeAdding()
    {
        CurrentTypeAddingCompleted = true;
    }

    public readonly void Report(IProgress<GachaLogFetchStatus> progress, bool isAuthKeyTimeout = false)
    {
        FetchStatus.AuthKeyTimeout = isAuthKeyTimeout;
        progress.Report(FetchStatus);
    }
}
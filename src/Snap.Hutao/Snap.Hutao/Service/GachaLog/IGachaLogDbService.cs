﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

internal interface IGachaLogDbService
{
    void AddGachaArchive(GachaArchive archive);

    ValueTask AddGachaArchiveAsync(GachaArchive archive);

    void AddGachaItemRange(List<GachaItem> items);

    ValueTask AddGachaItemsAsync(List<GachaItem> items);

    ValueTask RemoveGachaArchiveByIdAsync(Guid archiveId);

    void RemoveNewerGachaItemRangeByArchiveIdQueryTypeAndEndId(Guid archiveId, GachaType queryType, long endId);

    ValueTask<GachaArchive?> GetGachaArchiveByIdAsync(Guid archiveId, CancellationToken token);

    ValueTask<GachaArchive?> GetGachaArchiveByUidAsync(string uid, CancellationToken token);

    ObservableCollection<GachaArchive> GetGachaArchiveCollection();

    List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId);

    ValueTask<List<GachaItem>> GetGachaItemListByArchiveIdAsync(Guid archiveId);

    List<Web.Hutao.GachaLog.GachaItem> GetHutaoGachaItemList(Guid archiveId, GachaType queryType, long endId);

    long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);

    ValueTask<long> GetNewestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaType queryType, CancellationToken token);

    long GetOldestGachaItemIdByArchiveId(Guid archiveId);

    long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);

    ValueTask<long> GetOldestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaType queryType, CancellationToken token);

    ValueTask<long> GetNewestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaType queryType);

    ValueTask<long> GetOldestGachaItemIdByArchiveIdAsync(Guid archiveId);

    ValueTask<List<Web.Hutao.GachaLog.GachaItem>> GetHutaoGachaItemListAsync(Guid archiveId, GachaType queryType, long endId);

    ValueTask AddGachaItemRangeAsync(List<GachaItem> items);

    ValueTask RemoveNewerGachaItemRangeByArchiveIdQueryTypeAndEndIdAsync(Guid archiveId, GachaType queryType, long endId);
}
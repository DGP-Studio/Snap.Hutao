// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

internal interface IGachaLogDbService
{
    void AddGachaArchive(GachaArchive archive);

    ValueTask AddGachaArchiveAsync(GachaArchive archive);

    void AddGachaItems(List<GachaItem> items);

    ValueTask AddGachaItemsAsync(List<GachaItem> items);

    ValueTask DeleteGachaArchiveByIdAsync(Guid archiveId);

    void DeleteNewerGachaItemsByArchiveIdQueryTypeAndEndId(Guid archiveId, GachaConfigType queryType, long endId);

    ValueTask<GachaArchive?> GetGachaArchiveByUidAsync(string uid, CancellationToken token);

    ObservableCollection<GachaArchive> GetGachaArchiveCollection();

    List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId);

    List<Web.Hutao.GachaLog.GachaItem> GetHutaoGachaItemList(Guid archiveId, GachaConfigType queryType, long endId);

    long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaConfigType queryType);

    ValueTask<long> GetNewestGachaItemIdByArchiveIdAndQueryTypeAsync(Guid archiveId, GachaConfigType queryType, CancellationToken token);

    long GetOldestGachaItemIdByArchiveId(Guid archiveId);

    long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaConfigType queryType);
}
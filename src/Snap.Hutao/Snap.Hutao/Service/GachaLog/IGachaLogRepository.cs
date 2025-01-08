// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

internal interface IGachaLogRepository : IRepository<GachaArchive>, IRepository<GachaItem>
{
    void AddGachaArchive(GachaArchive archive);

    void AddGachaItemRange(IEnumerable<GachaItem> items);

    void RemoveGachaArchiveById(Guid archiveId);

    void RemoveGachaItemRangeByArchiveIdAndQueryTypeNewerThanEndId(Guid archiveId, GachaType queryType, long endId);

    GachaArchive? GetGachaArchiveById(Guid archiveId);

    GachaArchive? GetGachaArchiveByUid(string uid);

    ImmutableArray<string> GetGachaArchiveUidImmutableArray();

    ObservableCollection<GachaArchive> GetGachaArchiveCollection();

    ImmutableArray<GachaItem> GetGachaItemImmutableArrayByArchiveId(Guid archiveId);

    ImmutableArray<Web.Hutao.GachaLog.GachaItem> GetHutaoGachaItemListByArchiveIdAndQueryTypeNewerThanEndId(Guid archiveId, GachaType queryType, long endId);

    long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);

    long GetOldestGachaItemIdByArchiveId(Guid archiveId);

    long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);
}
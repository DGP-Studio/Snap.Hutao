// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.GachaLog;

internal interface IGachaLogDbService : IAppDbService<GachaArchive>, IAppDbService<GachaItem>
{
    void AddGachaArchive(GachaArchive archive);

    void AddGachaItemRange(List<GachaItem> items);

    void RemoveGachaArchiveById(Guid archiveId);

    void RemoveGachaItemRangeByArchiveIdAndQueryTypeNewerThanEndId(Guid archiveId, GachaType queryType, long endId);

    GachaArchive? GetGachaArchiveById(Guid archiveId);

    GachaArchive? GetGachaArchiveByUid(string uid);

    List<string> GetGachaArchiveUidList();

    ObservableCollection<GachaArchive> GetGachaArchiveCollection();

    List<GachaItem> GetGachaItemListByArchiveId(Guid archiveId);

    List<Web.Hutao.GachaLog.GachaItem> GetHutaoGachaItemListByArchiveIdAndQueryTypeNewerThanEndId(Guid archiveId, GachaType queryType, long endId);

    long GetNewestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);

    long GetOldestGachaItemIdByArchiveId(Guid archiveId);

    long GetOldestGachaItemIdByArchiveIdAndQueryType(Guid archiveId, GachaType queryType);
}
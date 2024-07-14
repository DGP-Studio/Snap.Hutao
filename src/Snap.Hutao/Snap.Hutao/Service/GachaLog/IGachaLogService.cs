// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Service.GachaLog.QueryProvider;
using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

/// <summary>
/// 祈愿记录服务
/// </summary>
[HighQuality]
internal interface IGachaLogService
{
    AdvancedDbCollectionView<GachaArchive>? Archives { get; }

    ValueTask<GachaArchive> EnsureArchiveInCollectionAsync(Guid archiveId, CancellationToken token = default(CancellationToken));

    ValueTask<GachaStatistics> GetStatisticsAsync(GachaArchive archive);

    ValueTask<List<GachaStatisticsSlim>> GetStatisticsSlimListAsync(CancellationToken token = default);

    ValueTask<bool> InitializeAsync(CancellationToken token = default);

    ValueTask<bool> RefreshGachaLogAsync(GachaLogQuery query, RefreshStrategyKind strategy, IProgress<GachaLogFetchStatus> progress, CancellationToken token);

    ValueTask RemoveArchiveAsync(GachaArchive archive);
}
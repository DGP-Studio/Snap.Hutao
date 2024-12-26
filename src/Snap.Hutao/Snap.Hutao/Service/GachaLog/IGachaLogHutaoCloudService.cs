// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.ViewModel.GachaLog;
using Snap.Hutao.Web.Hutao.GachaLog;
using Snap.Hutao.Web.Hutao.Response;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.GachaLog;

internal interface IGachaLogHutaoCloudService
{
    ValueTask<ValueResult<bool, string>> DeleteGachaItemsAsync(string uid, CancellationToken token = default);

    ValueTask<ValueResult<bool, HutaoStatistics>> GetCurrentEventStatisticsAsync(CancellationToken token = default);

    ValueTask<HutaoResponse<ImmutableArray<GachaEntry>>> GetGachaEntriesAsync(CancellationToken token = default);

    ValueTask<ValueResult<bool, Guid>> RetrieveGachaArchiveIdAsync(string uid, CancellationToken token = default);

    ValueTask<ValueResult<bool, string>> UploadGachaItemsAsync(GachaArchive gachaArchive, CancellationToken token = default);
}
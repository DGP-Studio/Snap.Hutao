// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.AvatarInfo.Factory;
using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoService
{
    ValueTask<Summary?> GetSummaryAsync(SummaryFactoryMetadataContext context, UserAndUid userAndUid, RefreshOptionKind refreshOptionKind, CancellationToken token = default);
}
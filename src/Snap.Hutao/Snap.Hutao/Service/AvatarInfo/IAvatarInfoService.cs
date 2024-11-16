// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.AvatarProperty;
using Snap.Hutao.ViewModel.User;

namespace Snap.Hutao.Service.AvatarInfo;

internal interface IAvatarInfoService
{
    ValueTask<ValueResult<RefreshResultKind, Summary?>> GetSummaryAsync(UserAndUid userAndUid, RefreshOption refreshOption, CancellationToken token = default);
}
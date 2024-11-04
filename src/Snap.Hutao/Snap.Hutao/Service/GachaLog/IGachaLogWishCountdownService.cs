// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.GachaLog;

namespace Snap.Hutao.Service.GachaLog;

internal interface IGachaLogWishCountdownService
{
    ValueTask<bool> InitializeAsync();

    ValueTask<ValueResult<bool, WishCountdowns>> GetWishCountdownsAsync();
}

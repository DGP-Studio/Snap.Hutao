// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

internal interface IGachaLogQueryProvider
{
    ValueTask<ValueResult<bool, GachaLogQuery>> GetQueryAsync();
}
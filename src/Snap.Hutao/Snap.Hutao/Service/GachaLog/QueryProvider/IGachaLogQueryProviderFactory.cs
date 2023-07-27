// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.GachaLog.QueryProvider;

internal interface IGachaLogQueryProviderFactory
{
    IGachaLogQueryProvider Create(RefreshOption option);
}
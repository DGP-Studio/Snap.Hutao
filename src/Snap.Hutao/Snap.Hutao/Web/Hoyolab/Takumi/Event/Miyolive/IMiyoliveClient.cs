// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Miyolive;

internal interface IMiyoliveClient
{
    ValueTask<Response<CodeListWrapper>> RefreshCodeAsync(string actId, CancellationToken token = default);
}
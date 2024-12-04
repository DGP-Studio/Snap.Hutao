// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Bbs.Home;

internal interface IHomeClient
{
    ValueTask<Response<NewHomeNewInfo>> GetNewHomeInfoAsync(int gid, CancellationToken token = default);
}
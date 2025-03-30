// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Downloader;

internal interface ISophonClient
{
    ValueTask<Response<SophonBuild>> GetBuildAsync(BranchWrapper branch, CancellationToken token = default);

    ValueTask<Response<SophonPatchBuild>> GetPatchBuildAsync(BranchWrapper branch, CancellationToken token = default);
}
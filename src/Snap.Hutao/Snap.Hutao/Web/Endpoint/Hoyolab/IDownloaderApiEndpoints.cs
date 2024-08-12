// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

internal interface IDownloaderApiEndpoints : IDownloaderApiRootAccess
{
    public string SophonChunkGetBuild(BranchWrapper branch)
    {
        return $"{DownloaderApiRoot}/sophon_chunk/api/getBuild?branch={branch.Branch}&package_id={branch.PackageId}&password={branch.Password}&tag={branch.Tag}";
    }
}
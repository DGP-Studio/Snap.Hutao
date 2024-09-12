// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;

namespace Snap.Hutao.Web.Endpoint.Hoyolab;

internal static class ApiEndpointsExtension
{
    public static string SophonChunkGetBuildByBranch(this IApiEndpoints apiEndpoints, BranchWrapper wrapper)
    {
        return string.Equals(wrapper.Branch, "PREDOWNLOAD", StringComparison.OrdinalIgnoreCase)
            ? apiEndpoints.SophonChunkGetBuildNoTag(wrapper)
            : apiEndpoints.SophonChunkGetBuild(wrapper);
    }
}
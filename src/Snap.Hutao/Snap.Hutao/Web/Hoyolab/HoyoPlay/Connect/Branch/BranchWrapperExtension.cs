// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;

internal static class BranchWrapperExtension
{
    public static BranchWrapper GetTaggedCopy(this BranchWrapper branchWrapper, string tag)
    {
        return new()
        {
            PackageId = branchWrapper.PackageId,
            Branch = branchWrapper.Branch,
            Password = branchWrapper.Password,
            Tag = tag,
        };
    }
}

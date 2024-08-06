// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint;

internal interface IInfrastructurePatchEndpoints : IInfrastructureRootAccess
{
    public string PatchYaeAchievement()
    {
        return $"{Root}/patch/yae";
    }

    public string PatchSnapHutao()
    {
        return $"{Root}/patch/hutao";
    }
}
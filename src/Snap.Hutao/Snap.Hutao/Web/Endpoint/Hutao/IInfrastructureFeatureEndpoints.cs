// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IInfrastructureFeatureEndpoints : IInfrastructureRootAccess
{
    public string Feature(string name)
    {
        return $"{Root}/client/{name}.json";
    }
}
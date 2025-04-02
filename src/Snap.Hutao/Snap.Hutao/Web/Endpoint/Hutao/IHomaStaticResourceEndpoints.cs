// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaStaticResourceEndpoints : IHomaRootAccess
{
    public string StaticResourceGetAcceleratedImageToken()
    {
        return $"{Root}/StaticResource/GetAcceleratedImageToken";
    }
}
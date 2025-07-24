// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IInfrastructureManagementEndpoints : IInfrastructureRawRootAccess
{
    public string AmIBanned()
    {
        return $"{RawRoot}/mgnt/am-i-banned";
    }
}
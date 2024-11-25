// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaDistributionEndpoints : IHomaRootAccess
{
    public string DistributionGetAcceleratedMirror(string filename)
    {
        return $"{Root}/Distribution/GetAcceleratedMirror?Filename={filename}";
    }
}
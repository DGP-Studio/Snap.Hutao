// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaDistributionEndpoints : IHomaRootAccess
{
    public string DistributionGetAccMirror(string filename)
    {
        return $"{Root}/Distribution/GetAccMirror?Filename={filename}";
    }
}
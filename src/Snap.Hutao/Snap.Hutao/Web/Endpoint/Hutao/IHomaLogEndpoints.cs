// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaLogEndpoints : IHomaRootAccess
{
    public string HutaoLogUpload()
    {
        return $"{Root}/HutaoLog/Upload";
    }
}
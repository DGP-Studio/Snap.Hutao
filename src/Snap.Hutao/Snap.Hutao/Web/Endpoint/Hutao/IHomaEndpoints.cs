// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaEndpoints :
    IHomaGachaLogEndpoints,
    IHomaServiceEndpoints,
    IHomaLogEndpoints,
    IHomaPassportEndpoints,
    IHomaSpiralAbyssEndpoints
{
    public string HomaWebsite(string path)
    {
        return $"{Root}/{path}";
    }
}
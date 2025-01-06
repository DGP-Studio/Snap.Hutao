// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IHomaRedeemCodeEndpoints : IHomaRootAccess
{
    public string RedeemCodeUse()
    {
        return $"{Root}/Redeem/Use";
    }
}
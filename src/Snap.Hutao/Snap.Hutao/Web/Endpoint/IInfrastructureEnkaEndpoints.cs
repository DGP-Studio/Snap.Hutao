// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;

namespace Snap.Hutao.Web.Endpoint;

internal interface IInfrastructureEnkaEndpoints : IInfrastructureRootAccess
{
    public string Enka(PlayerUid uid)
    {
        return $"{Root}/enka/{uid}";
    }

    public string EnkaPlayerInfo(PlayerUid uid)
    {
        return $"{Root}/enka/{uid}/info";
    }
}
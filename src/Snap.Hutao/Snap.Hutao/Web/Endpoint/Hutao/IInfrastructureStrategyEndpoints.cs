// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Endpoint.Hutao;

internal interface IInfrastructureStrategyEndpoints : IInfrastructureRootAccess
{
    public string StrategyAll()
    {
        return $"{Root}/strategy/all";
    }

    public string StrategyItem(AvatarId avatarId)
    {
        return $"{Root}/strategy/item?item_id={avatarId}";
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;

namespace Snap.Hutao.Service.Hutao;

internal interface IAvatarStrategyRepository : IRepository<AvatarStrategy>
{
    AvatarStrategy? GetStrategyByAvatarId(AvatarId avatarId);

    void AddStrategy(AvatarStrategy strategy);

    void RemoveStrategy(AvatarStrategy strategy);
}
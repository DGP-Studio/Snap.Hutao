// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Hutao;

internal interface IAvatarStrategyService
{
    ValueTask<AvatarStrategy?> GetStrategyByAvatarId(AvatarId avatarId);
}
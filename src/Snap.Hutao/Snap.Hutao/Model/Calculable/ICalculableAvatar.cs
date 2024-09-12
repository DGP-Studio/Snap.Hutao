// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculableAvatar : ICalculable
{
    AvatarId AvatarId { get; }

    uint LevelMin { get; }

    uint LevelMax { get; }

    List<ICalculableSkill> Skills { get; }
}
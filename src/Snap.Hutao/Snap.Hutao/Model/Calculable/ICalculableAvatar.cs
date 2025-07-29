// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculableAvatar : ICalculablePromoteLevel
{
    AvatarId AvatarId { get; }

    ImmutableArray<ICalculableSkill> Skills { get; }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class AvatarCollocationView
{
    public required AvatarId AvatarId { get; init; }

    public required ImmutableArray<AvatarView> Avatars { get; init; }

    public required ImmutableArray<WeaponView> Weapons { get; init; }

    public required ImmutableArray<ReliquarySetView> ReliquarySets { get; init; }
}
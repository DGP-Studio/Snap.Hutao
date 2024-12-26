// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class AvatarRankView
{
    public required string Floor { get; init; }

    public required ImmutableArray<AvatarView> Avatars { get; init; }
}
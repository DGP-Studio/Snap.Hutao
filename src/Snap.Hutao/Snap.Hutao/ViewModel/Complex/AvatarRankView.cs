// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class AvatarRankView
{
    public string Floor { get; set; } = default!;

    public List<AvatarView> Avatars { get; set; } = default!;
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class WeaponCollocationView
{
    public List<AvatarView> Avatars { get; set; } = default!;

    internal WeaponId WeaponId { get; set; }
}
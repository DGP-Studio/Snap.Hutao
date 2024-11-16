// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class AvatarCollocationView
{
    public AvatarId AvatarId { get; set; }

    public List<AvatarView> Avatars { get; set; } = default!;

    public List<WeaponView> Weapons { get; set; } = default!;

    public List<ReliquarySetView> ReliquarySets { get; set; } = default!;
}
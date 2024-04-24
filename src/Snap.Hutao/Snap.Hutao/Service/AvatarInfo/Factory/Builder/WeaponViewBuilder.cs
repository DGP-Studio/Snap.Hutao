// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AvatarInfo.Factory.Builder;

internal sealed class WeaponViewBuilder : IWeaponViewBuilder
{
    public ViewModel.AvatarProperty.WeaponView WeaponView { get; } = new();
}
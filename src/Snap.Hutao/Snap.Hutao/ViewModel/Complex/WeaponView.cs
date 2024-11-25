// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Metadata.Weapon;

namespace Snap.Hutao.ViewModel.Complex;

internal sealed class WeaponView : RateAndDelta, INameIcon<Uri>
{
    public WeaponView(Weapon weapon, double rate, double? lastRate)
        : base(rate, lastRate)
    {
        Name = weapon.Name;
        Icon = EquipIconConverter.IconNameToUri(weapon.Icon);
        Quality = weapon.Quality;
    }

    public string Name { get; }

    public Uri Icon { get; }

    public QualityType Quality { get; }
}
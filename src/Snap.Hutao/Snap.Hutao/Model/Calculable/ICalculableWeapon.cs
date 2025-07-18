// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Calculable;

internal interface ICalculableWeapon : ICalculablePromoteLevel
{
    WeaponId WeaponId { get; }
}
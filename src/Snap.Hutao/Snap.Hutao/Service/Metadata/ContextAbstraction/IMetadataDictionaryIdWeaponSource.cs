// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryIdWeaponSource
{
    public Dictionary<WeaponId, Model.Metadata.Weapon.Weapon> IdWeaponMap { get; set; }
}
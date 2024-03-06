// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Metadata.ContextAbstraction;

internal interface IMetadataDictionaryNameWeaponSource
{
    public Dictionary<string, Model.Metadata.Weapon.Weapon> NameWeaponMap { get; set; }
}
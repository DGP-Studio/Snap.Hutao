// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Metadata.Weapon;
using System.Collections.Immutable;

namespace Snap.Hutao.Service.Metadata.ContextAbstraction.ImmutableArray;

internal interface IMetadataArrayWeaponSource
{
    ImmutableArray<Weapon> Weapons { get; set; }
}
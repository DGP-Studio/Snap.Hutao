﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal interface ICultivationItemsAccess : INameAccess
{
    List<MaterialId> CultivationItems { get; }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Abstraction;

internal interface ICultivationItemsAccess : INameAccess
{
    ImmutableArray<MaterialId> CultivationItems { get; }
}
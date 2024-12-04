// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model;

internal interface IEntityAccess<out TEntity>
{
    TEntity Entity { get; }
}
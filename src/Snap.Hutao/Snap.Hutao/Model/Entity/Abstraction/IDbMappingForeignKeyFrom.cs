// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;

namespace Snap.Hutao.Model.Entity.Abstraction;

internal interface IDbMappingForeignKeyFrom<TSource, TFrom>
{
    [Pure]
    static abstract TSource From(in Guid foreignKey, in TFrom from);
}

internal interface IDbMappingForeignKeyFrom<TSource, T1, T2>
{
    [Pure]
    static abstract TSource From(in Guid foreignKey, in T1 param1, in T2 param2);
}
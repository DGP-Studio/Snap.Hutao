// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;

namespace Snap.Hutao.Model.Entity.Abstraction;

internal interface IDbMappingForeignKeyFrom<TSource, TFrom>
{
    [Pure]
    static abstract TSource From(Guid foreignKey, TFrom from);
}

internal interface IDbMappingForeignKeyFrom<TSource, T1, T2>
{
    [Pure]
    static abstract TSource From(Guid foreignKey, T1 param1, T2 param2);
}

internal interface IDbMappingForeignKeyFrom<TSource, T1, T2, T3>
{
    [Pure]
    static abstract TSource From(Guid foreignKey, T1 param1, T2 param2, T3 param3);
}
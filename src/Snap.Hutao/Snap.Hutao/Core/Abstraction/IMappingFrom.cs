// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics.Contracts;

namespace Snap.Hutao.Core.Abstraction;

internal interface IMappingFrom<TSelf, TFrom>
    where TSelf : IMappingFrom<TSelf, TFrom>
{
    [Pure]
    static abstract TSelf From(TFrom source);
}

internal interface IMappingFrom<TSelf, T1, T2>
    where TSelf : IMappingFrom<TSelf, T1, T2>
{
    [Pure]
    static abstract TSelf From(T1 t1, T2 t2);
}

internal interface IMappingFrom<TSelf, T1, T2, T3>
    where TSelf : IMappingFrom<TSelf, T1, T2, T3>
{
    [Pure]
    static abstract TSelf From(T1 t1, T2 t2, T3 t3);
}
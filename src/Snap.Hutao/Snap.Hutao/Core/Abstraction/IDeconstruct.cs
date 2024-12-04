// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

internal interface IDeconstruct<T1, T2>
{
    void Deconstruct(out T1 t1, out T2 t2);
}

internal interface IDeconstruct<T1, T2, T3>
{
    void Deconstruct(out T1 t1, out T2 t2, out T3 t3);
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Threading.Abstraction;

internal interface IAwaitable<out TAwaiter>
    where TAwaiter : IAwaiter
{
    TAwaiter GetAwaiter();
}

internal interface IAwaitable<out TAwaiter, out TResult>
    where TAwaiter : IAwaiter<TResult>
{
    TAwaiter GetAwaiter();
}
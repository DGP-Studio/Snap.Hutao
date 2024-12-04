// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Abstraction;

internal interface IPinnable<TData>
{
    ref TData GetPinnableReference();
}
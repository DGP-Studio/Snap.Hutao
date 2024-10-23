// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Snap.Hutao.Win32.Foundation;

internal unsafe struct FlexibleArray<TElement>
    where TElement : unmanaged
{
    public TElement* Reference;

    public ref TElement this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Reference[index];
    }
}
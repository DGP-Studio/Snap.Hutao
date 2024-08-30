// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Extension;

internal ref struct RefTuple<T1, T2>
{
    public ref T1 Item1;
    public ref T2 Item2;

    public RefTuple(ref T1 item1, ref T2 item2)
    {
        Item1 = ref item1;
        Item2 = ref item2;
    }
}
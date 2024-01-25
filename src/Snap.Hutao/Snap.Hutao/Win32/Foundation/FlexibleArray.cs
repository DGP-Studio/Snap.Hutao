// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal unsafe struct FlexibleArray<T>
    where T : unmanaged
{
    public T* Reference;
}
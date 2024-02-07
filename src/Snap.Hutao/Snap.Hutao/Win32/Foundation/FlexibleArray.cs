// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Foundation;

internal unsafe struct FlexibleArray<TElement>
    where TElement : unmanaged
{
    public TElement* Reference;
}
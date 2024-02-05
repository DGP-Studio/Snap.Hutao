// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Foundation.Collections;

namespace Snap.Hutao.Control.Collection.AdvancedCollectionView;

internal sealed class VectorChangedEventArgs : IVectorChangedEventArgs
{
    public VectorChangedEventArgs(CollectionChange cc, int index = -1, object item = null!)
    {
        CollectionChange = cc;
        Index = (uint)index;
    }

    public CollectionChange CollectionChange { get; }

    public uint Index { get; }
}

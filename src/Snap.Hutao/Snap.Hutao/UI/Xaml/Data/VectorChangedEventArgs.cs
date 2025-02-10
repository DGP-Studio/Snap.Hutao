// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Foundation.Collections;

namespace Snap.Hutao.UI.Xaml.Data;

internal sealed partial class VectorChangedEventArgs : IVectorChangedEventArgs
{
    public VectorChangedEventArgs(CollectionChange change, int index = -1)
    {
        CollectionChange = change;
        Index = (uint)index;
    }

    public CollectionChange CollectionChange { get; }

    public uint Index { get; }
}
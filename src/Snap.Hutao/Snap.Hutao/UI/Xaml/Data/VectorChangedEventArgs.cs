// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Windows.Foundation.Collections;

namespace Snap.Hutao.UI.Xaml.Data;

internal sealed partial class VectorChangedEventArgs : IVectorChangedEventArgs
{
    public VectorChangedEventArgs(CollectionChange cc, int index = -1, object item = default!)
    {
        CollectionChange = cc;
        Index = (uint)index;
    }

    public CollectionChange CollectionChange { get; }

    public uint Index { get; }
}

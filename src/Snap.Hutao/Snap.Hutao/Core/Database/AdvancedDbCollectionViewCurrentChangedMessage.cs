// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.Database;

internal sealed class AdvancedDbCollectionViewCurrentChangedMessage<TItem>
    where TItem : class
{
    public AdvancedDbCollectionViewCurrentChangedMessage(TItem? currentItem)
    {
        CurrentItem = currentItem;
    }

    public TItem? CurrentItem { get; }
}
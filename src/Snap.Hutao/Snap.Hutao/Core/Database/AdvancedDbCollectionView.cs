// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Core.Database;

internal sealed class AdvancedDbCollectionView<TEntity> : AdvancedCollectionView<TEntity>
    where TEntity : class, IAdvancedCollectionViewItem, ISelectable
{
    public AdvancedDbCollectionView(IList<TEntity> source)
        : base(source)
    {
    }
}
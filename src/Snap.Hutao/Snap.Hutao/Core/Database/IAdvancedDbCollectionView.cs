// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.Core.Database;

internal interface IAdvancedDbCollectionView<TEntity> : IAdvancedCollectionView<TEntity>
    where TEntity : class, IPropertyValuesProvider
{
    IDisposable SuppressChangeCurrentItem();
}
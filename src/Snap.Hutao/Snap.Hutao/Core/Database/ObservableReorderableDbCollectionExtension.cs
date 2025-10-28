// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.UI.Xaml.Data;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

internal static class ObservableReorderableDbCollectionExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntity> ToObservableReorderableDbCollection<TEntity>(this IEnumerable<TEntity> source, IServiceProvider serviceProvider)
        where TEntity : class, IReorderable
    {
        return source is List<TEntity> list
            ? new(list, serviceProvider)
            : new([.. source], serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntityOnly, TEntity> ToObservableReorderableDbCollection<TEntityOnly, TEntity>(this IEnumerable<TEntityOnly> source, IServiceProvider serviceProvider)
        where TEntityOnly : class, IEntityAccess<TEntity>
        where TEntity : class, IReorderable
    {
        return source is List<TEntityOnly> list
            ? new(list, serviceProvider)
            : new([.. source], serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AdvancedDbCollectionView<TEntityOnly, TEntity> ToAdvancedDbCollectionViewWrappedObservableReorderableDbCollection<TEntityOnly, TEntity>(this IEnumerable<TEntityOnly> source, IServiceProvider serviceProvider)
        where TEntityOnly : class, IPropertyValuesProvider, IEntityAccess<TEntity>
        where TEntity : class, ISelectable, IReorderable
    {
        return new(ToObservableReorderableDbCollection<TEntityOnly, TEntity>(source, serviceProvider), serviceProvider);
    }
}
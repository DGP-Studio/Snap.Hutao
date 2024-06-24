// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

internal static class ObservableReorderableDbCollectionExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntity> ToObservableReorderableDbCollection<TEntity>(this IEnumerable<TEntity> source, IServiceProvider serviceProvider)
        where TEntity : class, IReorderable
    {
        return source is List<TEntity> list
            ? new ObservableReorderableDbCollection<TEntity>(list, serviceProvider)
            : new ObservableReorderableDbCollection<TEntity>([.. source], serviceProvider);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ObservableReorderableDbCollection<TEntityOnly, TEntity> ToObservableReorderableDbCollection<TEntityOnly, TEntity>(this IEnumerable<TEntityOnly> source, IServiceProvider serviceProvider)
        where TEntityOnly : class, IEntityAccess<TEntity>
        where TEntity : class, IReorderable
    {
        return source is List<TEntityOnly> list
            ? new ObservableReorderableDbCollection<TEntityOnly, TEntity>(list, serviceProvider)
            : new ObservableReorderableDbCollection<TEntityOnly, TEntity>([.. source], serviceProvider);
    }
}
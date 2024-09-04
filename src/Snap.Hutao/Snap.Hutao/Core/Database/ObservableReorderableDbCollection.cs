// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Database;

internal sealed class ObservableReorderableDbCollection<TEntity> : ObservableCollection<TEntity>
    where TEntity : class, IReorderable
{
    private readonly IServiceProvider serviceProvider;

    public ObservableReorderableDbCollection(List<TEntity> items, IServiceProvider serviceProvider)
        : base(AdjustIndex(items.SortBy(x => x.Index)))
    {
        this.serviceProvider = serviceProvider;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Add:
                OnReorder();
                break;
        }
    }

    private static List<TEntity> AdjustIndex(List<TEntity> list)
    {
        Span<TEntity> span = CollectionsMarshal.AsSpan(list);
        for (int i = 0; i < span.Length; i++)
        {
            ref readonly TEntity item = ref span[i];
            item.Index = i;
        }

        return list;
    }

    private void OnReorder()
    {
        AdjustIndex((List<TEntity>)Items);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            DbSet<TEntity> dbSet = appDbContext.Set<TEntity>();
            foreach (ref readonly TEntity item in CollectionsMarshal.AsSpan((List<TEntity>)Items))
            {
                dbSet.UpdateAndSave(item);
            }
        }
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class ObservableReorderableDbCollection<TEntityAccess, TEntity> : ObservableCollection<TEntityAccess>
    where TEntityAccess : class, IEntityAccess<TEntity>
    where TEntity : class, IReorderable
{
    private readonly IServiceProvider serviceProvider;

    public ObservableReorderableDbCollection(List<TEntityAccess> items, IServiceProvider serviceProvider)
        : base(AdjustIndex(items.SortBy(x => x.Entity.Index)))
    {
        this.serviceProvider = serviceProvider;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Add:
                OnReorder();
                break;
        }
    }

    private static List<TEntityAccess> AdjustIndex(List<TEntityAccess> list)
    {
        Span<TEntityAccess> span = CollectionsMarshal.AsSpan(list);
        for (int i = 0; i < span.Length; i++)
        {
            ref readonly TEntityAccess item = ref span[i];
            item.Entity.Index = i;
        }

        return list;
    }

    private void OnReorder()
    {
        AdjustIndex((List<TEntityAccess>)Items);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            DbSet<TEntity> dbSet = appDbContext.Set<TEntity>();
            foreach (ref readonly TEntityAccess item in CollectionsMarshal.AsSpan((List<TEntityAccess>)Items))
            {
                dbSet.UpdateAndSave(item.Entity);
            }
        }
    }
}
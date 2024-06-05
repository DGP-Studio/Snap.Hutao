// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Microsoft.EntityFrameworkCore;
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

    public IAdvancedCollectionView? View { get; set; }

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
        for (int i = 0; i < list.Count; i++)
        {
            ref readonly TEntity item = ref span[i];
            item.Index = i;
        }

        return list;
    }

    private void OnReorder()
    {
        using (View?.DeferRefresh())
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
}

[SuppressMessage("", "SA1402")]
internal sealed class ObservableReorderableDbCollection<TEntityOnly, TEntity> : ObservableCollection<TEntityOnly>
    where TEntityOnly : class, IEntityAccess<TEntity>
    where TEntity : class, IReorderable
{
    private readonly IServiceProvider serviceProvider;

    public ObservableReorderableDbCollection(List<TEntityOnly> items, IServiceProvider serviceProvider)
        : base(AdjustIndex(items.SortBy(x => x.Entity.Index)))
    {
        this.serviceProvider = serviceProvider;
    }

    public IAdvancedCollectionView? View { get; set; }

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

    private static List<TEntityOnly> AdjustIndex(List<TEntityOnly> list)
    {
        Span<TEntityOnly> span = CollectionsMarshal.AsSpan(list);
        for (int i = 0; i < list.Count; i++)
        {
            ref readonly TEntityOnly item = ref span[i];
            item.Entity.Index = i;
        }

        return list;
    }

    private void OnReorder()
    {
        using (View?.DeferRefresh())
        {
            AdjustIndex((List<TEntityOnly>)Items);

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                DbSet<TEntity> dbSet = appDbContext.Set<TEntity>();
                foreach (ref readonly TEntityOnly item in CollectionsMarshal.AsSpan((List<TEntityOnly>)Items))
                {
                    dbSet.UpdateAndSave(item.Entity);
                }
            }
        }
    }
}
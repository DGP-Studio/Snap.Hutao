// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Core.Database;

[SuppressMessage("", "SA1649")]
file static class Sort
{
    public static List<TEntity> ByIndex<TEntity>(List<TEntity> items)
        where TEntity : class, IReorderable
    {
        items.Sort((x, y) => x.Index.CompareTo(y.Index));
        return items;
    }

    public static List<TEntityAccess> ByIndex<TEntityAccess, TEntity>(List<TEntityAccess> items)
        where TEntityAccess : class, IEntityAccess<TEntity>
        where TEntity : class, IReorderable
    {
        items.Sort((x, y) => x.Entity.Index.CompareTo(y.Entity.Index));
        return items;
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class ObservableReorderableDbCollection<TEntity> : ObservableCollection<TEntity>
    where TEntity : class, IReorderable
{
    private readonly IServiceProvider serviceProvider;

    public ObservableReorderableDbCollection(List<TEntity> items, IServiceProvider serviceProvider)
        : base(AdjustIndex(Sort.ByIndex(items))) // Normalized index
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
        // Input list can have non-contiguous indices, so we need to normalize them
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Index = i;
        }

        return list;
    }

    private void OnReorder()
    {
        AdjustIndex(Unsafe.As<List<TEntity>>(Items));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Set<TEntity>().UpdateRangeAndSave(Items);
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
        : base(AdjustIndex(Sort.ByIndex<TEntityAccess, TEntity>(items)))
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
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Entity.Index = i;
        }

        return list;
    }

    private static TEntity AccessEntity(TEntityAccess access)
    {
        return access.Entity;
    }

    private void OnReorder()
    {
        AdjustIndex(Unsafe.As<List<TEntityAccess>>(Items));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<AppDbContext>().Set<TEntity>().UpdateRangeAndSave(Items.Select(AccessEntity));
        }
    }
}
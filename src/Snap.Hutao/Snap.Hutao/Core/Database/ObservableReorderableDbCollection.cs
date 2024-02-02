// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Snap.Hutao.Model.Entity.Database;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Database;

internal sealed class ObservableReorderableDbCollection<T> : ObservableCollection<T>
    where T : class, IReorderable
{
    private readonly IServiceProvider serviceProvider;

    public ObservableReorderableDbCollection(List<T> items, IServiceProvider serviceProvider)
        : base(AdjustIndex(items))
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

    private static List<T> AdjustIndex(List<T> list)
    {
        Span<T> span = CollectionsMarshal.AsSpan(list);
        for (int i = 0; i < list.Count; i++)
        {
            ref readonly T item = ref span[i];
            item.Index = i;
        }

        return list;
    }

    private void OnReorder()
    {
        AdjustIndex((List<T>)Items);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            DbSet<T> dbSet = appDbContext.Set<T>();
            foreach (ref readonly T item in CollectionsMarshal.AsSpan((List<T>)Items))
            {
                dbSet.UpdateAndSave(item);
            }
        }
    }
}
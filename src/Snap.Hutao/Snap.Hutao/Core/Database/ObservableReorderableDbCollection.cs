// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Core.Database;

internal sealed class ObservableReorderableDbCollection<T> : ObservableCollection<T>
    where T : class, IReorderable
{
    private readonly DbContext dbContext;
    private bool previousChangeIsRemoved;

    public ObservableReorderableDbCollection(List<T> items, DbContext dbContext)
        : base(AdjustIndex(items))
    {
        this.dbContext = dbContext;
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
                previousChangeIsRemoved = true;
                break;
            case NotifyCollectionChangedAction.Add:
                if (!previousChangeIsRemoved)
                {
                    return;
                }

                OnReorder();
                previousChangeIsRemoved = false;
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

        DbSet<T> dbSet = dbContext.Set<T>();
        foreach (ref readonly T item in CollectionsMarshal.AsSpan((List<T>)Items))
        {
            dbSet.UpdateAndSave(item);
        }
    }
}
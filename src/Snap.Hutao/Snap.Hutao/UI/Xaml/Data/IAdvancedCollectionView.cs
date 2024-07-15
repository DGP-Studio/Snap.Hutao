// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace Snap.Hutao.UI.Xaml.Data;

internal interface IAdvancedCollectionView<T> : ICollectionView, IEnumerable
    where T : class
{
    object? ICollectionView.CurrentItem
    {
        get => CurrentItem;
    }

    new T? CurrentItem { get; }

    Predicate<T>? Filter { get; set; }

    ObservableCollection<SortDescription> SortDescriptions { get; }

    IList<T> SourceCollection { get; }

    object IList<object>.this[int index]
    {
        get => this[index];
        set => this[index] = (T)value;
    }

    new T this[int index] { get; set; }

    void ICollection<object>.Add(object item)
    {
        Add((T)item);
    }

    void Add(T item);

    bool ICollection<object>.Contains(object item)
    {
        return Contains((T)item);
    }

    bool Contains(T item);

    void ICollection<object>.CopyTo(object[] array, int arrayIndex)
    {
        CopyTo((T[])array, arrayIndex);
    }

    void CopyTo(T[] array, int arrayIndex);

    IDisposable DeferRefresh();

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
        return GetEnumerator();
    }

    new IEnumerator<T> GetEnumerator();

    int IList<object>.IndexOf(object item)
    {
        if (item is T dataItem1)
        {
            return IndexOf(dataItem1);
        }

        // WinUI somehow pass in a FrameworkElement with DataContext as actual item
        if (item is FrameworkElement { DataContext: T dataItem2 })
        {
            return IndexOf(dataItem2);
        }

        return IndexOf(default!);
    }

    int IndexOf(T item);

    void IList<object>.Insert(int index, object item)
    {
        Insert(index, (T)item);
    }

    void Insert(int index, T item);

    bool ICollectionView.MoveCurrentTo(object item)
    {
        return MoveCurrentTo((T)item);
    }

    bool MoveCurrentTo(T? item);

    void Refresh();

    void RefreshFilter();

    void RefreshSorting();

    bool ICollection<object>.Remove(object item)
    {
        return Remove((T)item);
    }

    bool Remove(T item);
}
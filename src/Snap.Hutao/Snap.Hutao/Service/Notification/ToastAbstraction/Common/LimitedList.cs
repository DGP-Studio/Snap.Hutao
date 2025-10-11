// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections;

namespace Snap.Hutao.Service.Notification.ToastAbstraction.Common;

internal sealed class LimitedList<T> : IList<T>
{
    private readonly List<T> list;

    public LimitedList(int limit)
    {
        list = new List<T>(limit);

        Limit = limit;
    }

    public int Limit { get; }

    public int Count { get => list.Count; }

    public bool IsReadOnly { get => false; }

    public T this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }

    public void Add(T item)
    {
        if (list.Count >= Limit)
        {
            throw new InvalidOperationException("This list is limited to " + Limit + " items. You cannot add more items.");
        }

        list.Add(item);
    }

    public void Clear()
    {
        list.Clear();
    }

    public bool Contains(T item)
    {
        return list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        list.Insert(index, item);
    }

    public bool Remove(T item)
    {
        return list.Remove(item);
    }

    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
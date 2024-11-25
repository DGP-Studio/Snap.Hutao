// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Helpers;
using Snap.Hutao.Core.ExceptionService;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

internal sealed partial class InterspersedObservableCollection : IList, IEnumerable<object>, INotifyCollectionChanged
{
    private readonly Dictionary<int, object> interspersedObjects = [];
    private bool isInsertingOriginal;

    public InterspersedObservableCollection()
        : this(new ObservableCollection<object>())
    {
    }

    public InterspersedObservableCollection(object itemsSource)
    {
        if (itemsSource is not IList list)
        {
            throw new ArgumentException("The input items source must implements System.Collections.IList");
        }

        ItemsSource = list;

        if (ItemsSource is not INotifyCollectionChanged incc)
        {
            throw new ArgumentException("The input items source must implements System.Collections.Specialized.INotifyCollectionChanged");
        }

        WeakEventListener<InterspersedObservableCollection, object?, NotifyCollectionChangedEventArgs> weakPropertyChangedListener = new(this)
        {
            OnEventAction = static (instance, source, eventArgs) => instance.OnCollectionChanged(source, eventArgs),
            OnDetachAction = weakEventListener => incc.CollectionChanged -= weakEventListener.OnEvent, // Use Local Reference Only
        };
        incc.CollectionChanged += weakPropertyChangedListener.OnEvent;
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public IList ItemsSource { get; }

    public bool IsFixedSize
    {
        get => false;
    }

    public bool IsReadOnly
    {
        get => false;
    }

    public int Count
    {
        get => ItemsSource.Count + interspersedObjects.Count;
    }

    public bool IsSynchronized
    {
        get => false;
    }

    public object SyncRoot
    {
        get => new();
    }

    public object? this[int index]
    {
        get => interspersedObjects.TryGetValue(index, out object? value) ? value :

            // Find out the number of elements in our dictionary with keys below ours.
            ItemsSource[ToInnerIndex(index)];
        set => throw new NotImplementedException();
    }

    public void Insert(int index, object? obj)
    {
        // Move existing keys at index over to make room for new item
        MoveKeysForward(index, 1);

        ArgumentNullException.ThrowIfNull(obj);
        interspersedObjects[index] = obj;

        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, obj, index));
    }

    public void InsertAt(int outerIndex, object obj)
    {
        // Find out our closest index based on interspersed keys
        // Note: we exclude the = from ToInnerIndex here
        int index = outerIndex - interspersedObjects.Keys.Count(key => key < outerIndex);

        // If we're inserting where we would normally, then just do that, otherwise we need extra room to not move other keys
        if (index != outerIndex)
        {
            // Skip over until the current spot unlike normal
            MoveKeysForward(outerIndex, 1);

            // Prevent Collection callback from moving keys forward on insert
            isInsertingOriginal = true;
        }

        // Insert into original collection
        ItemsSource.Insert(index, obj);
    }

    public IEnumerator<object> GetEnumerator()
    {
        // Index of our current 'virtual' position
        int i = 0;
        int count = 0;
        int realized = 0;

        foreach (object element in ItemsSource)
        {
            while (interspersedObjects.TryGetValue(i++, out object? obj))
            {
                // Track interspersed items used
                realized++;

                yield return obj;
            }

            // Track original items used
            count++;

            yield return element;
        }

        // Add any remaining items in our interspersed collection past the index we reached in the original collection
        if (realized < interspersedObjects.Count)
        {
            // Only select items past our current index, but make sure we've sorted them by their index as well.
            foreach ((int _, object value) in interspersedObjects.Where(kvp => kvp.Key >= i).OrderBy(kvp => kvp.Key))
            {
                yield return value;
            }
        }
    }

    public int Add(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        int index = ItemsSource.Add(value);
        return ToOuterIndex(index);
    }

    public void Clear()
    {
        ItemsSource.Clear();
        interspersedObjects.Clear();
    }

    public bool Contains(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return interspersedObjects.ContainsValue(value) || ItemsSource.Contains(value);
    }

    public int IndexOf(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (ItemKeySearch(value, out int key))
        {
            return key;
        }

        int index = ItemsSource.IndexOf(value);

        // Find out the number of elements in our dictionary with keys below ours.
        return index == -1 ? -1 : ToOuterIndex(index);
    }

    public void Remove(object? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (ItemKeySearch(value, out int key))
        {
            interspersedObjects.Remove(key);

            // Move other interspersed items back
            MoveKeysBackward(key, 1);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, key));

            return;
        }

        ItemsSource.Remove(value);
    }

    public void RemoveAt(int index)
    {
        HutaoException.NotSupported();
    }

    public void CopyTo(Array array, int index)
    {
        HutaoException.NotSupported();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void OnCollectionChanged(object? source, NotifyCollectionChangedEventArgs eventArgs)
    {
        switch (eventArgs.Action)
        {
            case NotifyCollectionChangedAction.Add:
                // Shift any existing interspersed items after the inserted item
                ArgumentNullException.ThrowIfNull(eventArgs.NewItems);
                int count = eventArgs.NewItems.Count;

                if (count > 0)
                {
                    if (!isInsertingOriginal)
                    {
                        MoveKeysForward(eventArgs.NewStartingIndex, count);
                    }

                    isInsertingOriginal = false;

                    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, eventArgs.NewItems, ToOuterIndex(eventArgs.NewStartingIndex)));
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                ArgumentNullException.ThrowIfNull(eventArgs.OldItems);
                count = eventArgs.OldItems.Count;

                if (count > 0)
                {
                    int outerIndex = ToOuterIndexAfterRemoval(eventArgs.OldStartingIndex);

                    MoveKeysBackward(outerIndex, count);

                    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, eventArgs.OldItems, outerIndex));
                }

                break;
            case NotifyCollectionChangedAction.Reset:

                ReadjustKeys();

                // TODO: ListView doesn't like this notification and throws a visual tree duplication exception...
                // Not sure what to do with that yet...
                CollectionChanged?.Invoke(this, eventArgs);
                break;
        }
    }

    private void MoveKeysForward(int pivot, int amount)
    {
        // Sort in reverse order to work from highest to lowest
        foreach (int key in interspersedObjects.Keys.OrderByDescending(v => v))
        {
            // If it's the last item in the collection, we still want to move our last key, otherwise we'd use <=
            if (key < pivot)
            {
                break;
            }

            interspersedObjects[key + amount] = interspersedObjects[key];
            interspersedObjects.Remove(key);
        }
    }

    private void MoveKeysBackward(int pivot, int amount)
    {
        // Sort in regular order to work from the earliest indices onwards
        foreach (int key in interspersedObjects.Keys.OrderBy(v => v))
        {
            // Skip elements before the pivot point
            // Include pivot point as that's the point where we start modifying beyond
            if (key <= pivot)
            {
                continue;
            }

            interspersedObjects[key - amount] = interspersedObjects[key];
            interspersedObjects.Remove(key);
        }
    }

    private void ReadjustKeys()
    {
        int count = ItemsSource.Count;
        int existing = 0;

        foreach (int key in interspersedObjects.Keys.OrderBy(v => v))
        {
            if (key <= count)
            {
                existing++;
                continue;
            }

            interspersedObjects[count + existing++] = interspersedObjects[key];
            interspersedObjects.Remove(key);
        }
    }

    private int ToInnerIndex(int outerIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(outerIndex, Count);

        if (interspersedObjects.ContainsKey(outerIndex))
        {
            throw new ArgumentException("The outer index can't be inserted as a key to the original collection.");
        }

        return outerIndex - interspersedObjects.Keys.Count(key => key <= outerIndex);
    }

    private int ToOuterIndex(int innerIndex)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(innerIndex, ItemsSource.Count);

        foreach ((int key, object _) in interspersedObjects.OrderBy(v => v.Key))
        {
            if (innerIndex >= key)
            {
                innerIndex++;
                continue;
            }

            break;
        }

        return innerIndex;
    }

    private int ToOuterIndexAfterRemoval(int innerIndexToProject)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(innerIndexToProject, ItemsSource.Count + 1);

        // TODO: Deal with bounds (0 / Count)? Or is it the same?
        foreach ((int key, object _) in interspersedObjects.OrderBy(v => v.Key))
        {
            if (innerIndexToProject >= key)
            {
                innerIndexToProject++;
                continue;
            }

            break;
        }

        return innerIndexToProject;
    }

    private bool ItemKeySearch(object value, out int key)
    {
        if (interspersedObjects.ContainsValue(value))
        {
            key = value is null
                ? interspersedObjects.First(kvp => kvp.Value is null).Key
                : interspersedObjects.First(kvp => kvp.Value.Equals(value)).Key;

            return true;
        }

        key = default;
        return false;
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Helpers;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;

internal sealed class InterspersedObservableCollection : IList, IEnumerable<object>, INotifyCollectionChanged
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
            throw new ArgumentException("The input items source must be assignable to the System.Collections.IList type.");
        }

        ItemsSource = list;

        if (ItemsSource is INotifyCollectionChanged notifier)
        {
            WeakEventListener<InterspersedObservableCollection, object?, NotifyCollectionChangedEventArgs> weakPropertyChangedListener = new(this)
            {
                OnEventAction = static (instance, source, eventArgs) => instance.OnCollectionChanged(source, eventArgs),
                OnDetachAction = (weakEventListener) => notifier.CollectionChanged -= weakEventListener.OnEvent, // Use Local Reference Only
            };
            notifier.CollectionChanged += weakPropertyChangedListener.OnEvent;
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public IList ItemsSource { get; private set; }

    public bool IsFixedSize => false;

    public bool IsReadOnly => false;

    public int Count => ItemsSource.Count + interspersedObjects.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => new();

    public object? this[int index]
    {
        get
        {
            if (interspersedObjects.TryGetValue(index, out object? value))
            {
                return value;
            }

            // Find out the number of elements in our dictionary with keys below ours.
            return ItemsSource[ToInnerIndex(index)];
        }
        set => throw new NotImplementedException();
    }

    public void Insert(int index, object? obj)
    {
        MoveKeysForward(index, 1); // Move existing keys at index over to make room for new item

        ArgumentNullException.ThrowIfNull(obj);
        interspersedObjects[index] = obj;

        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, obj, index));
    }

    public void InsertAt(int outerIndex, object obj)
    {
        // Find out our closest index based on interspersed keys
        int index = outerIndex - interspersedObjects.Keys.Count(key => key < outerIndex); // Note: we exclude the = from ToInnerIndex here

        // If we're inserting where we would normally, then just do that, otherwise we need extra room to not move other keys
        if (index != outerIndex)
        {
            MoveKeysForward(outerIndex, 1); // Skip over until the current spot unlike normal

            isInsertingOriginal = true; // Prevent Collection callback from moving keys forward on insert
        }

        // Insert into original collection
        ItemsSource.Insert(index, obj);

        // TODO: handle manipulation/notification if not observable
    }

    public IEnumerator<object> GetEnumerator()
    {
        int i = 0; // Index of our current 'virtual' position
        int count = 0;
        int realized = 0;

        foreach (object element in ItemsSource)
        {
            while (interspersedObjects.TryGetValue(i++, out object? obj))
            {
                realized++; // Track interspersed items used

                yield return obj;
            }

            count++; // Track original items used

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
        int index = ItemsSource.Add(value); //// TODO: If the collection isn't observable, we should do manipulations/notifications here...?
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

            MoveKeysBackward(key, 1); // Move other interspersed items back

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, key));

            return;
        }

        ItemsSource.Remove(value);
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
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
            if (key < pivot) //// If it's the last item in the collection, we still want to move our last key, otherwise we'd use <=
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
            if (key <= pivot) //// Include pivot point as that's the point where we start modifying beyond
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

        //// TODO: Deal with bounds (0 / Count)? Or is it the same?

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

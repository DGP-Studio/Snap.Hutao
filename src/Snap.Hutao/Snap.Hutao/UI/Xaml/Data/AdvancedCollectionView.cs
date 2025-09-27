// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Core;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using NotifyCollectionChangedAction = System.Collections.Specialized.NotifyCollectionChangedAction;

namespace Snap.Hutao.UI.Xaml.Data;

internal partial class AdvancedCollectionView<T> : IAdvancedCollectionView<T>, INotifyPropertyChanged, ISupportIncrementalLoading, IComparer<T>
    where T : class, IPropertyValuesProvider
{
    private readonly bool created;
    private readonly List<T> view = [];

    private Predicate<T>? filter;
    private int deferCounter;
    private WeakEventListener<AdvancedCollectionView<T>, object?, NotifyCollectionChangedEventArgs>? sourceWeakEventListener;

    public AdvancedCollectionView(IList<T> source)
    {
        SortDescriptions = [];
        SortDescriptions.CollectionChanged += SortDescriptionsCollectionChanged;

        Source = source;
        created = true;
    }

    public event EventHandler<object>? CurrentChanged;

    public event CurrentChangingEventHandler? CurrentChanging;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event VectorChangedEventHandler<object>? VectorChanged;

    public int Count { get => view.Count; }

    public bool IsReadOnly { get => Source.IsReadOnly; }

    public IObservableVector<object> CollectionGroups { get => default!; }

    public T? CurrentItem
    {
        get => IndexInBounds(CurrentPosition, view) ? view[CurrentPosition] : default;
        set => MoveCurrentTo(value);
    }

    public int CurrentPosition { get; private set; }

    public bool HasMoreItems { get => Source is ISupportIncrementalLoading { HasMoreItems: true }; }

    public bool IsCurrentAfterLast { get => CurrentPosition >= view.Count; }

    public bool IsCurrentBeforeFirst { get => CurrentPosition < 0; }

    public Predicate<T>? Filter
    {
        get => filter;
        set
        {
            // Delegate equality check, cannot use ReferenceEquals
            if (filter == value)
            {
                return;
            }

            filter = value;
            HandleFilterChanged();
        }
    }

    public ObservableCollection<SortDescription> SortDescriptions { get; }

    public IReadOnlyList<T> View
    {
        // Prevent down casting, we really don't expect the user to modify the view
        get => view.AsReadOnly();
    }

    [field: MaybeNull]
    public IList<T> Source
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            if (field is not null)
            {
                DetachPropertyChangedHandler(field);
            }

            field = value;
            AttachPropertyChangedHandler(field);

            sourceWeakEventListener?.Detach();

            if (field is INotifyCollectionChanged sourceINCC)
            {
                sourceWeakEventListener = new(this)
                {
                    OnEventAction = OnSourceNotifyCollectionCollectionChanged,
                    OnDetachAction = listener => sourceINCC.CollectionChanged -= listener.OnEvent,
                };
                sourceINCC.CollectionChanged += sourceWeakEventListener.OnEvent;
            }

            HandleSourceChanged();
            OnPropertyChanged();

            static void OnSourceNotifyCollectionCollectionChanged(AdvancedCollectionView<T> @this, object? source, NotifyCollectionChangedEventArgs args)
            {
                @this.SourceNotifyCollectionChangedCollectionChanged(args);
            }
        }
    }

    public T this[int index]
    {
        get => view[index];
        set => view[index] = value;
    }

    public void Refresh()
    {
        HandleSourceChanged();
    }

    public void RefreshFilter()
    {
        HandleFilterChanged();
    }

    public void RefreshSorting()
    {
        HandleSortChanged();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return view.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return view.GetEnumerator();
    }

    public void Add(T? item)
    {
#pragma warning disable SH007
        Source.Add(item!);
#pragma warning restore SH007
    }

    public void Clear()
    {
        Source.Clear();
    }

    public bool Contains(T? item)
    {
#pragma warning disable SH007
        return view.Contains(item!);
#pragma warning restore SH007
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        view.CopyTo(array, arrayIndex);
    }

    public bool Remove(T? item)
    {
#pragma warning disable SH007
        return Source.Remove(item!);
#pragma warning restore SH007
    }

    public int IndexOf(T? item)
    {
#pragma warning disable SH007
        return view.IndexOf(item!);
#pragma warning restore SH007
    }

    public void Insert(int index, T? item)
    {
#pragma warning disable SH007
        Source.Insert(index, item!);
#pragma warning restore SH007
    }

    public void RemoveAt(int index)
    {
        Remove(view[index]);
    }

    public bool MoveCurrentTo(T? item)
    {
        return (item is not null && ReferenceEquals(item, CurrentItem)) || MoveCurrentToIndex(IndexOf(item));
    }

    public bool MoveCurrentToPosition(int index)
    {
        return MoveCurrentToIndex(index);
    }

    public bool MoveCurrentToFirst()
    {
        return MoveCurrentToIndex(0);
    }

    public bool MoveCurrentToLast()
    {
        return MoveCurrentToIndex(view.Count - 1);
    }

    public bool MoveCurrentToNext()
    {
        return MoveCurrentToIndex(CurrentPosition + 1);
    }

    public bool MoveCurrentToPrevious()
    {
        return MoveCurrentToIndex(CurrentPosition - 1);
    }

    public IAsyncOperation<LoadMoreItemsResult>? LoadMoreItemsAsync(uint count)
    {
        return (Source as ISupportIncrementalLoading)?.LoadMoreItemsAsync(count);
    }

    public IDisposable DeferRefresh()
    {
        return new NotificationDeferrer(this);
    }

    int IComparer<T>.Compare(T? x, T? y)
    {
        foreach (SortDescription sd in SortDescriptions)
        {
            object? cx, cy;

            if (string.IsNullOrEmpty(sd.PropertyName))
            {
                cx = x;
                cy = y;
            }
            else
            {
                cx = x?.GetPropertyValue(sd.PropertyName);
                cy = y?.GetPropertyValue(sd.PropertyName);
            }

            int cmp = sd.Comparer.Compare(cx, cy);

            if (cmp is not 0)
            {
                return sd.Direction is SortDirection.Ascending ? +cmp : -cmp;
            }
        }

        return 0;
    }

    protected virtual void OnCurrentChangedOverride()
    {
    }

    private static bool IndexInBounds(int index, IList<T> list)
    {
        return index > -1 && index < list.Count;
    }

    private void ItemOnPropertyChanged(object? item, PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(item);
        T typedItem = Unsafe.As<T>(item);

        if (!(filter?.Invoke(typedItem) ?? true) /* NotInView */ ||
            /* InView! */ SortDescriptions.All(sd => sd.PropertyName != e.PropertyName) /* NoSortProperty */)
        {
            // NotInView || NoSortProperty
            return;
        }

        // Here should always be in view
        int oldViewIndex = view.IndexOf(typedItem);

        // Check if item is in view
        if (oldViewIndex < 0)
        {
            // According to previous logic, this should never happen
            Debugger.Break();
            return;
        }

        view.RemoveAt(oldViewIndex);
        int newViewIndex = view.BinarySearch(typedItem, comparer: this);
        if (newViewIndex < 0)
        {
            newViewIndex = ~newViewIndex;
        }

        // Only trigger expensive UI updates if the index really changed
        if (newViewIndex != oldViewIndex)
        {
            bool itemWasCurrent = oldViewIndex == CurrentPosition;
            OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, oldViewIndex));

            view.Insert(newViewIndex, typedItem);

            OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, newViewIndex));

            // Restore current position if it was the CurrentItem that changed
            _ = !itemWasCurrent || MoveCurrentToIndex(newViewIndex);
        }
        else
        {
            // Index didn't change, just insert back
            view.Insert(newViewIndex, typedItem);
        }
    }

    private void AttachPropertyChangedHandler(IEnumerable? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (object item in items)
        {
            if (item is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged += ItemOnPropertyChanged;
            }
        }
    }

    private void DetachPropertyChangedHandler(IEnumerable? items)
    {
        if (items is null)
        {
            return;
        }

        foreach (object item in items)
        {
            if (item is INotifyPropertyChanged notifyPropertyChanged)
            {
                notifyPropertyChanged.PropertyChanged -= ItemOnPropertyChanged;
            }
        }
    }

    private void HandleSortChanged()
    {
        view.Sort(this);
        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
    }

    private void HandleFilterChanged()
    {
        if (filter is not null)
        {
            for (int index = 0; index < view.Count; index++)
            {
                T item = view[index];
                if (filter(item))
                {
                    continue;
                }

                RemoveFromView(index);
                index--;
            }
        }

        // The view is sorted, we should enumerate the Source in order,
        // and insert the item in the correct position
        HashSet<T> viewSet = [.. view];
        int viewIndex = 0;
        foreach ((int index, T item) in Source.Order(this).Index())
        {
            if (viewSet.Contains(item))
            {
                viewIndex++;
                continue;
            }

            if (HandleSourceItemAdded(index, item, viewIndex))
            {
                viewIndex++;
            }
        }
    }

    private void HandleSourceChanged()
    {
        T? currentItem = CurrentItem;
        view.Clear();

        if (filter is null)
        {
            view.AddRange(Source);
        }
        else
        {
            foreach (T item in Source)
            {
                if (!filter(item))
                {
                    continue;
                }

                view.Add(item);
            }
        }

        if (SortDescriptions.Count > 0)
        {
            view.Sort(this);
        }

        view.TrimExcess();

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
        MoveCurrentTo(currentItem);
    }

    private void SourceNotifyCollectionChangedCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AttachPropertyChangedHandler(e.NewItems);
                if (deferCounter > 0)
                {
                    break;
                }

                if (e.NewItems is [{ } newItem])
                {
                    HandleSourceItemAdded(e.NewStartingIndex, Unsafe.As<T>(newItem));
                }
                else
                {
                    HandleSourceChanged();
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                DetachPropertyChangedHandler(e.OldItems);
                if (deferCounter > 0)
                {
                    break;
                }

                if (e.OldItems is [{ } oldItem])
                {
                    HandleSourceItemRemoved(e.OldStartingIndex, Unsafe.As<T>(oldItem));
                }
                else
                {
                    HandleSourceChanged();
                }

                break;
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Reset:
                if (deferCounter > 0)
                {
                    break;
                }

                HandleSourceChanged();

                break;
        }
    }

    private bool HandleSourceItemAdded(int newStartingIndex, T newItem, int? viewIndex = null)
    {
        // Filtered out
        if (filter is not null && !filter(newItem))
        {
            return false;
        }

        // There are two cases remaining:
        // 1.Filter is null
        // 2.Filter is not null, but the item is not filtered out
        int newViewIndex = newStartingIndex;

        // Find the index where the item should be inserted
        if (SortDescriptions.Count > 0)
        {
            newViewIndex = view.BinarySearch(newItem, this);
            if (newViewIndex < 0)
            {
                newViewIndex = ~newViewIndex;
            }
        }
        else
        {
            // No sort descriptions
            // Situation 2: Filter is not null, but the item is not filtered out
            if (filter is not null)
            {
                // It's the first item
                if (newStartingIndex == 0 || view.Count == 0)
                {
                    newViewIndex = 0;
                }

                // It's the last item
                else if (newStartingIndex == Source.Count - 1)
                {
                    newViewIndex = view.Count;
                }

                // View index is provided
                else if (viewIndex.HasValue)
                {
                    newViewIndex = viewIndex.Value;
                }
                else
                {
                    // It's in the middle, and view index is not provided
                    // Perform an O(n) search
                    for (int peekSource = 0, peekView = 0; peekSource < Source.Count; peekSource++)
                    {
                        if (peekSource == newStartingIndex)
                        {
                            newViewIndex = peekView;
                            break;
                        }

                        if (Equals(view[peekView], Source[peekSource]))
                        {
                            peekView++;
                        }
                    }
                }
            }

            // Filter is null, we can just insert at the newStartingIndex
        }

        view.Insert(newViewIndex, newItem);
        if (newViewIndex <= CurrentPosition)
        {
            CurrentPosition++;
        }

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, newViewIndex));
        return true;
    }

    private void HandleSourceItemRemoved(int oldStartingIndex, T oldItem)
    {
        if (filter is not null && !filter(oldItem))
        {
            return;
        }

        // Element must be reference type
        if (!IndexInBounds(oldStartingIndex, view) || !ReferenceEquals(View[oldStartingIndex], oldItem))
        {
            oldStartingIndex = view.IndexOf(oldItem);
        }

        if (oldStartingIndex < 0)
        {
            return;
        }

        RemoveFromView(oldStartingIndex);
    }

    private void RemoveFromView(int itemIndex)
    {
        if (itemIndex == CurrentPosition)
        {
            // Current item is removed
            MoveCurrentToIndex(-1);
        }

        view.RemoveAt(itemIndex);

        if (itemIndex < CurrentPosition)
        {
            // Item before current is removed
            CurrentPosition--;
        }

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, itemIndex));
    }

    private void SortDescriptionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (deferCounter > 0)
        {
            return;
        }

        HandleSortChanged();
    }

    private bool MoveCurrentToIndex(int index)
    {
        if (index < -1 || index >= view.Count)
        {
            return false;
        }

        if (index == CurrentPosition)
        {
            return false;
        }

        // If ACV is not created, we don't cancel the execution,
        // CurrentPosition should be set during construction.
        OnCurrentChanging(out bool cancel);
        if (cancel)
        {
            return false;
        }

        CurrentPosition = index;
        OnCurrentChanged();
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = default!)
    {
        if (!created)
        {
            return;
        }

        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private void OnCurrentChanging(out bool cancel)
    {
        if (!created || deferCounter > 0)
        {
            cancel = false;
            return;
        }

        CurrentChangingEventArgs e = new();
        CurrentChanging?.Invoke(this, e);
        cancel = e.Cancel;
    }

    private void OnCurrentChanged()
    {
        if (!created || deferCounter > 0)
        {
            return;
        }

        OnCurrentChangedOverride();
        try
        {
            CurrentChanged?.Invoke(this, default!);
        }
        catch (Exception ex)
        {
            // E_FAIL 0x80004005 can happen when moving items around
            ex.Data["CollectionElementType"] = TypeNameHelper.GetTypeDisplayName(typeof(T));
            throw;
        }

        OnPropertyChanged(nameof(CurrentItem));
    }

    private void OnVectorChanged(IVectorChangedEventArgs e)
    {
        if (!created || deferCounter > 0)
        {
            return;
        }

        VectorChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(Count));
    }

    internal sealed partial class NotificationDeferrer : IDisposable
    {
        private readonly AdvancedCollectionView<T> advancedCollectionView;
        private readonly T? currentItem;

        public NotificationDeferrer(AdvancedCollectionView<T> advancedCollectionView)
        {
            this.advancedCollectionView = advancedCollectionView;
            currentItem = this.advancedCollectionView.CurrentItem;
            this.advancedCollectionView.deferCounter++;
        }

        public void Dispose()
        {
            advancedCollectionView.MoveCurrentTo(currentItem);
            advancedCollectionView.deferCounter--;
            advancedCollectionView.Refresh();
        }
    }
}
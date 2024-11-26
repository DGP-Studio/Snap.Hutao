// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using NotifyCollectionChangedAction = System.Collections.Specialized.NotifyCollectionChangedAction;

namespace Snap.Hutao.UI.Xaml.Data;

internal partial class AdvancedCollectionView<T> : IAdvancedCollectionView<T>, INotifyPropertyChanged, ISupportIncrementalLoading, IComparer<T>
    where T : class, IAdvancedCollectionViewItem
{
    private readonly bool created;

    private IList<T> source;
    private Predicate<T>? filter;
    private int deferCounter;
    private WeakEventListener<AdvancedCollectionView<T>, object?, NotifyCollectionChangedEventArgs>? sourceWeakEventListener;

    public AdvancedCollectionView(IList<T> source)
    {
        View = [];
        SortDescriptions = [];
        SortDescriptions.CollectionChanged += SortDescriptionsCollectionChanged;
        Source = source;

        created = true;
    }

    public event EventHandler<object>? CurrentChanged;

    public event CurrentChangingEventHandler? CurrentChanging;

    public event PropertyChangedEventHandler? PropertyChanged;

    public event VectorChangedEventHandler<object>? VectorChanged;

    public int Count
    {
        get => View.Count;
    }

    [Obsolete("IsReadOnly is not supported")]
    public bool IsReadOnly { get => source is null; }

    public IObservableVector<object> CollectionGroups
    {
        get => default!;
    }

    public T? CurrentItem
    {
        get => CurrentPosition > -1 && CurrentPosition < View.Count ? View[CurrentPosition] : default;
        set => MoveCurrentTo(value);
    }

    public int CurrentPosition { get; private set; }

    public bool HasMoreItems { get => source is ISupportIncrementalLoading { HasMoreItems: true }; }

    public bool IsCurrentAfterLast { get => CurrentPosition >= View.Count; }

    public bool IsCurrentBeforeFirst { get => CurrentPosition < 0; }

    public Predicate<T>? Filter
    {
        get => filter;
        set
        {
            if (filter == value)
            {
                return;
            }

            filter = value;
            HandleFilterChanged();
        }
    }

    public ObservableCollection<SortDescription> SortDescriptions { get; }

    public IList<T> SourceCollection { get => source; }

    public List<T> View { get; }

    private IList<T> Source
    {
        get => source;

        [MemberNotNull(nameof(source))]
        set
        {
            if (ReferenceEquals(source, value))
            {
                return;
            }

            if (source is not null)
            {
                DetachPropertyChangedHandler(source);
            }

            source = value;
            AttachPropertyChangedHandler(source);

            sourceWeakEventListener?.Detach();

            if (source is INotifyCollectionChanged sourceINCC)
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

            static void OnSourceNotifyCollectionCollectionChanged(AdvancedCollectionView<T> target, object? source, NotifyCollectionChangedEventArgs args)
            {
                target.SourceNotifyCollectionChangedCollectionChanged(args);
            }
        }
    }

    public T this[int index]
    {
        get => View[index];
        set => View[index] = value;
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
        return View.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return View.GetEnumerator();
    }

    public void Add(T item)
    {
        source.Add(item);
    }

    public void Clear()
    {
        source.Clear();
    }

    public bool Contains(T item)
    {
        return View.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        View.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return source.Remove(item);
    }

    public int IndexOf(T item)
    {
        return View.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        source.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Remove(View[index]);
    }

    [SuppressMessage("", "SH007")]
    public bool MoveCurrentTo(T? item)
    {
        return (item is not null && item.Equals(CurrentItem)) || MoveCurrentToIndex(IndexOf(item!));
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
        return MoveCurrentToIndex(View.Count - 1);
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
        return (source as ISupportIncrementalLoading)?.LoadMoreItemsAsync(count);
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

    private void OnPropertyChanged([CallerMemberName] string propertyName = default!)
    {
        if (!created)
        {
            return;
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ItemOnPropertyChanged(object? item, PropertyChangedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(item);
        T typedItem = (T)item;

        if (!(filter?.Invoke(typedItem) ?? true) || SortDescriptions.All(sd => sd.PropertyName != e.PropertyName))
        {
            return;
        }

        int oldIndex = View.IndexOf(typedItem);

        // Check if item is in view
        if (oldIndex < 0)
        {
            return;
        }

        View.RemoveAt(oldIndex);
        int targetIndex = View.BinarySearch(typedItem, comparer: this);
        if (targetIndex < 0)
        {
            targetIndex = ~targetIndex;
        }

        // Only trigger expensive UI updates if the index really changed
        if (targetIndex != oldIndex)
        {
            bool itemWasCurrent = oldIndex == CurrentPosition;
            OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, oldIndex, typedItem));

            View.Insert(targetIndex, typedItem);

            OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, targetIndex, typedItem));

            // Restore current position if it was the CurrentItem that changed
            _ = !itemWasCurrent || MoveCurrentToIndex(targetIndex);
        }
        else
        {
            View.Insert(targetIndex, typedItem);
        }
    }

    private void AttachPropertyChangedHandler(IEnumerable items)
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

    private void DetachPropertyChangedHandler(IEnumerable items)
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
        View.Sort(this);
        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
    }

    private void HandleFilterChanged()
    {
        if (filter is not null)
        {
            for (int index = 0; index < View.Count; index++)
            {
                T item = View[index];
                if (filter(item))
                {
                    continue;
                }

                RemoveFromView(index, item);
                index--;
            }
        }

        HashSet<T> viewSet = new(View);
        int viewIndex = 0;
        for (int index = 0; index < source.Count; index++)
        {
            T item = source[index];
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
        View.Clear();
        View.TrimExcess();

        if (filter is null && SortDescriptions.Count <= 0)
        {
            // Fast path
            View.AddRange(Source);
        }
        else
        {
            foreach (T item in Source)
            {
                if (filter is not null && !filter(item))
                {
                    continue;
                }

                if (SortDescriptions.Count > 0)
                {
                    int targetIndex = View.BinarySearch(item, this);
                    if (targetIndex < 0)
                    {
                        targetIndex = ~targetIndex;
                    }

                    View.Insert(targetIndex, item);
                }
                else
                {
                    View.Add(item);
                }
            }
        }

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
        MoveCurrentTo(currentItem);
    }

    private void SourceNotifyCollectionChangedCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                ArgumentNullException.ThrowIfNull(e.NewItems);
                AttachPropertyChangedHandler(e.NewItems);
                if (deferCounter > 0)
                {
                    break;
                }

                if (e.NewItems?.Count is 1)
                {
                    object? newItem = e.NewItems[0];
                    ArgumentNullException.ThrowIfNull(newItem);
                    HandleSourceItemAdded(e.NewStartingIndex, (T)newItem);
                }
                else
                {
                    HandleSourceChanged();
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                ArgumentNullException.ThrowIfNull(e.OldItems);
                DetachPropertyChangedHandler(e.OldItems);
                if (deferCounter > 0)
                {
                    break;
                }

                if (e.OldItems?.Count == 1)
                {
                    object? oldItem = e.OldItems[0];
                    ArgumentNullException.ThrowIfNull(oldItem);
                    HandleSourceItemRemoved(e.OldStartingIndex, (T)oldItem);
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
        if (filter is not null && !filter(newItem))
        {
            return false;
        }

        int newViewIndex = newStartingIndex;

        if (SortDescriptions.Count > 0)
        {
            newViewIndex = View.BinarySearch(newItem, this);
            if (newViewIndex < 0)
            {
                newViewIndex = ~newViewIndex;
            }
        }
        else if (filter is not null)
        {
            if (source is null)
            {
                HandleSourceChanged();
                return false;
            }

            if (newStartingIndex == 0 || View.Count == 0)
            {
                newViewIndex = 0;
            }
            else if (newStartingIndex == source.Count - 1)
            {
                newViewIndex = View.Count;
            }
            else if (viewIndex.HasValue)
            {
                newViewIndex = viewIndex.Value;
            }
            else
            {
                for (int i = 0, j = 0; i < source.Count; i++)
                {
                    if (i == newStartingIndex)
                    {
                        newViewIndex = j;
                        break;
                    }

                    if (Equals(View[j], source[i]))
                    {
                        j++;
                    }
                }
            }
        }

        View.Insert(newViewIndex, newItem);
        if (newViewIndex <= CurrentPosition)
        {
            CurrentPosition++;
        }

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, newViewIndex, newItem));
        return true;
    }

    private void HandleSourceItemRemoved(int oldStartingIndex, T oldItem)
    {
        if (filter is not null && !filter(oldItem))
        {
            return;
        }

        if (oldStartingIndex < 0 || oldStartingIndex >= View.Count || !Equals(View[oldStartingIndex], oldItem))
        {
            oldStartingIndex = View.IndexOf(oldItem);
        }

        if (oldStartingIndex < 0)
        {
            return;
        }

        RemoveFromView(oldStartingIndex, oldItem);
    }

    private void RemoveFromView(int itemIndex, T item)
    {
        View.RemoveAt(itemIndex);
        if (itemIndex <= CurrentPosition)
        {
            CurrentPosition--;

            // Removed item is last item
            if (View.Count == itemIndex)
            {
                OnCurrentChanged();
            }
        }

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, itemIndex, item));
    }

    private void SortDescriptionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (deferCounter > 0)
        {
            return;
        }

        HandleSortChanged();
    }

    private bool MoveCurrentToIndex(int i)
    {
        if (i == CurrentPosition)
        {
            return false;
        }

        if (i < -1 || i >= View.Count)
        {
            // view is empty, i is 0, current pos is -1
            OnPropertyChanged(nameof(CurrentItem));
            return false;
        }

        OnCurrentChanging(out bool cancel);
        if (cancel)
        {
            return false;
        }

        CurrentPosition = i;
        OnCurrentChanged();
        return true;
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
        CurrentChanged?.Invoke(this, default!);
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

        public NotificationDeferrer(AdvancedCollectionView<T> acvs)
        {
            advancedCollectionView = acvs;
            currentItem = advancedCollectionView.CurrentItem;
            advancedCollectionView.deferCounter++;
        }

        public void Dispose()
        {
            advancedCollectionView.MoveCurrentTo(currentItem);
            advancedCollectionView.deferCounter--;
            advancedCollectionView.Refresh();
        }
    }
}
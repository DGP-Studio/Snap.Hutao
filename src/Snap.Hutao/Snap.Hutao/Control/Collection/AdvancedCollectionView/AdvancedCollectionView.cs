// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Collections;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Data;
using Snap.Hutao.Core.ExceptionService;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using NotifyCollectionChangedAction = System.Collections.Specialized.NotifyCollectionChangedAction;

namespace Snap.Hutao.Control.Collection.AdvancedCollectionView;

internal sealed class AdvancedCollectionView : IAdvancedCollectionView, INotifyPropertyChanged, ISupportIncrementalLoading, IComparer<object>
{
    private readonly List<object> view;
    private readonly ObservableCollection<SortDescription> sortDescriptions;
    private readonly Dictionary<string, PropertyInfo> sortProperties;
    private readonly bool liveShapingEnabled;
    private readonly HashSet<string> observedFilterProperties = [];

    private IList source;
    private Predicate<object>? filter;
    private int deferCounter;
    private WeakEventListener<AdvancedCollectionView, object?, NotifyCollectionChangedEventArgs>? sourceWeakEventListener;

    public AdvancedCollectionView()
        : this(new List<object>(0))
    {
    }

    public AdvancedCollectionView(IList source, bool isLiveShaping = false)
    {
        liveShapingEnabled = isLiveShaping;
        view = [];
        sortDescriptions = [];
        sortDescriptions.CollectionChanged += SortDescriptionsCollectionChanged;
        sortProperties = [];
        Source = source;
    }

    public event VectorChangedEventHandler<object>? VectorChanged;

    public event EventHandler<object>? CurrentChanged;

    public event CurrentChangingEventHandler? CurrentChanging;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IList Source
    {
        get => source;

        [MemberNotNull(nameof(source))]
        set
        {
            if (source == value)
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

            if (source is INotifyCollectionChanged sourceNcc)
            {
                sourceWeakEventListener =
                    new WeakEventListener<AdvancedCollectionView, object?, NotifyCollectionChangedEventArgs>(this)
                    {
                        // Call the actual collection changed event
                        OnEventAction = (source, changed, arg3) => SourceNotifyCollectionChangedCollectionChanged(source, arg3),

                        // The source doesn't exist anymore
                        OnDetachAction = (listener) =>
                        {
                            ArgumentNullException.ThrowIfNull(sourceWeakEventListener);
                            sourceNcc.CollectionChanged -= sourceWeakEventListener.OnEvent;
                        },
                    };
                sourceNcc.CollectionChanged += sourceWeakEventListener.OnEvent;
            }

            HandleSourceChanged();
            OnPropertyChanged();
        }
    }

    public int Count
    {
        get => view.Count;
    }

    public bool IsReadOnly
    {
        get => source is null || source.IsReadOnly;
    }

    public IObservableVector<object> CollectionGroups
    {
        get => default!;
    }

    public object? CurrentItem
    {
        get => CurrentPosition > -1 && CurrentPosition < view.Count ? view[CurrentPosition] : null;
        set => MoveCurrentTo(value);
    }

    public int CurrentPosition { get; private set; }

    public bool HasMoreItems
    {
        get => (source as ISupportIncrementalLoading)?.HasMoreItems ?? false;
    }

    public bool IsCurrentAfterLast
    {
        get => CurrentPosition >= view.Count;
    }

    public bool IsCurrentBeforeFirst
    {
        get => CurrentPosition < 0;
    }

    public bool CanFilter
    {
        get => true;
    }

    public Predicate<object> Filter
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

    public bool CanSort
    {
        get => true;
    }

    public IList<SortDescription> SortDescriptions
    {
        get => sortDescriptions;
    }

    public IEnumerable SourceCollection
    {
        get => source;
    }

    public object this[int index]
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

    public IEnumerator<object> GetEnumerator()
    {
        return view.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return view.GetEnumerator();
    }

    public void Add(object item)
    {
        ThrowHelper.NotSupportedIf(IsReadOnly, "Collection is read-only.");
        source.Add(item);
    }

    public void Clear()
    {
        ThrowHelper.NotSupportedIf(IsReadOnly, "Collection is read-only.");
        source.Clear();
    }

    public bool Contains(object item)
    {
        return view.Contains(item);
    }

    public void CopyTo(object[] array, int arrayIndex)
    {
        view.CopyTo(array, arrayIndex);
    }

    public bool Remove(object item)
    {
        ThrowHelper.NotSupportedIf(IsReadOnly, "Collection is read-only.");
        source.Remove(item);
        return true;
    }

    public int IndexOf(object item)
    {
        return view.IndexOf(item);
    }

    public void Insert(int index, object item)
    {
        ThrowHelper.NotSupportedIf(IsReadOnly, "Collection is read-only.");
        source.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        Remove(view[index]);
    }

    public bool MoveCurrentTo(object? item)
    {
        return item == CurrentItem || MoveCurrentToIndex(IndexOf(item));
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
        return (source as ISupportIncrementalLoading)?.LoadMoreItemsAsync(count);
    }

    public void ObserveFilterProperty(string propertyName)
    {
        observedFilterProperties.Add(propertyName);
    }

    public void ClearObservedFilterProperties()
    {
        observedFilterProperties.Clear();
    }

    public IDisposable DeferRefresh()
    {
        return new NotificationDeferrer(this);
    }

    int IComparer<object>.Compare(object? x, object? y)
    {
        if (sortProperties.Count <= 0)
        {
            Type? listType = source?.GetType();
            Type? type;

            if (listType is { IsGenericType: true })
            {
                type = listType.GetGenericArguments()[0];
            }
            else
            {
                type = x?.GetType();
            }

            foreach (SortDescription sd in sortDescriptions)
            {
                if (!string.IsNullOrEmpty(sd.PropertyName))
                {
                    sortProperties[sd.PropertyName] = type?.GetProperty(sd.PropertyName);
                }
            }
        }

        foreach (SortDescription sd in sortDescriptions)
        {
            object cx, cy;

            if (string.IsNullOrEmpty(sd.PropertyName))
            {
                cx = x;
                cy = y;
            }
            else
            {
                PropertyInfo pi = sortProperties[sd.PropertyName];

                cx = pi.GetValue(x!);
                cy = pi.GetValue(y!);
            }

            int cmp = sd.Comparer.Compare(cx, cy);

            if (cmp != 0)
            {
                return sd.Direction == SortDirection.Ascending ? +cmp : -cmp;
            }
        }

        return 0;
    }

    internal void OnPropertyChanged([CallerMemberName] string propertyName = default!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ItemOnPropertyChanged(object? item, PropertyChangedEventArgs e)
    {
        if (!liveShapingEnabled)
        {
            return;
        }

        bool? filterResult = filter?.Invoke(item);

        if (filterResult.HasValue && observedFilterProperties.Contains(e.PropertyName))
        {
            int viewIndex = view.IndexOf(item);
            if (viewIndex != -1 && !filterResult.Value)
            {
                RemoveFromView(viewIndex, item);
            }
            else if (viewIndex == -1 && filterResult.Value)
            {
                int index = source.IndexOf(item);
                HandleItemAdded(index, item);
            }
        }

        if ((filterResult ?? true) && SortDescriptions.Any(sd => sd.PropertyName == e.PropertyName))
        {
            int oldIndex = view.IndexOf(item);

            // Check if item is in view:
            if (oldIndex < 0)
            {
                return;
            }

            view.RemoveAt(oldIndex);
            int targetIndex = view.BinarySearch(item, this);
            if (targetIndex < 0)
            {
                targetIndex = ~targetIndex;
            }

            // Only trigger expensive UI updates if the index really changed:
            if (targetIndex != oldIndex)
            {
                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemRemoved, oldIndex, item));

                view.Insert(targetIndex, item);

                OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, targetIndex, item));
            }
            else
            {
                view.Insert(targetIndex, item);
            }
        }
        else if (string.IsNullOrEmpty(e.PropertyName))
        {
            HandleSourceChanged();
        }
    }

    private void AttachPropertyChangedHandler(IEnumerable items)
    {
        if (!liveShapingEnabled || items is null)
        {
            return;
        }

        foreach (INotifyPropertyChanged item in items.OfType<INotifyPropertyChanged>())
        {
            item.PropertyChanged += ItemOnPropertyChanged;
        }
    }

    private void DetachPropertyChangedHandler(IEnumerable items)
    {
        if (!liveShapingEnabled || items is null)
        {
            return;
        }

        foreach (INotifyPropertyChanged item in items.OfType<INotifyPropertyChanged>())
        {
            item.PropertyChanged -= ItemOnPropertyChanged;
        }
    }

    private void HandleSortChanged()
    {
        sortProperties.Clear();
        view.Sort(this);
        sortProperties.Clear();
        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
    }

    private void HandleFilterChanged()
    {
        if (filter is not null)
        {
            for (int index = 0; index < view.Count; index++)
            {
                object item = view.ElementAt(index);
                if (filter(item))
                {
                    continue;
                }

                RemoveFromView(index, item);
                index--;
            }
        }

        HashSet<object> viewHash = new(view);
        int viewIndex = 0;
        for (int index = 0; index < source.Count; index++)
        {
            object? item = source[index];
            ArgumentNullException.ThrowIfNull(item);
            if (viewHash.Contains(item))
            {
                viewIndex++;
                continue;
            }

            if (HandleItemAdded(index, item, viewIndex))
            {
                viewIndex++;
            }
        }
    }

    private void HandleSourceChanged()
    {
        sortProperties.Clear();
        object? currentItem = CurrentItem;
        view.Clear();
        foreach (object? item in Source)
        {
            if (filter is not null && !filter(item))
            {
                continue;
            }

            if (sortDescriptions.Any())
            {
                int targetIndex = view.BinarySearch(item, this);
                if (targetIndex < 0)
                {
                    targetIndex = ~targetIndex;
                }

                view.Insert(targetIndex, item);
            }
            else
            {
                view.Add(item);
            }
        }

        sortProperties.Clear();
        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.Reset));
        MoveCurrentTo(currentItem);
    }

    private void SourceNotifyCollectionChangedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                ArgumentNullException.ThrowIfNull(e.NewItems);
                AttachPropertyChangedHandler(e.NewItems);
                if (deferCounter <= 0)
                {
                    if (e.NewItems?.Count == 1)
                    {
                        object? newItem = e.NewItems[0];
                        ArgumentNullException.ThrowIfNull(newItem);
                        HandleItemAdded(e.NewStartingIndex, newItem);
                    }
                    else
                    {
                        HandleSourceChanged();
                    }
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                ArgumentNullException.ThrowIfNull(e.OldItems);
                DetachPropertyChangedHandler(e.OldItems);
                if (deferCounter <= 0)
                {
                    if (e.OldItems?.Count == 1)
                    {
                        object? oldItem = e.OldItems[0];
                        ArgumentNullException.ThrowIfNull(oldItem);
                        HandleItemRemoved(e.OldStartingIndex, oldItem);
                    }
                    else
                    {
                        HandleSourceChanged();
                    }
                }

                break;
            case NotifyCollectionChangedAction.Move:
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Reset:
                if (deferCounter <= 0)
                {
                    HandleSourceChanged();
                }

                break;
        }
    }

    private bool HandleItemAdded(int newStartingIndex, object newItem, int? viewIndex = null)
    {
        if (filter is not null && !filter(newItem))
        {
            return false;
        }

        int newViewIndex = view.Count;

        if (sortDescriptions.Any())
        {
            sortProperties.Clear();
            newViewIndex = view.BinarySearch(newItem, this);
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

            if (newStartingIndex == 0 || view.Count == 0)
            {
                newViewIndex = 0;
            }
            else if (newStartingIndex == source.Count - 1)
            {
                newViewIndex = view.Count;
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

                    if (view[j] == source[i])
                    {
                        j++;
                    }
                }
            }
        }

        view.Insert(newViewIndex, newItem);
        if (newViewIndex <= CurrentPosition)
        {
            CurrentPosition++;
        }

        OnVectorChanged(new VectorChangedEventArgs(CollectionChange.ItemInserted, newViewIndex, newItem));
        return true;
    }

    private void HandleItemRemoved(int oldStartingIndex, object oldItem)
    {
        if (filter is not null && !filter(oldItem))
        {
            return;
        }

        if (oldStartingIndex < 0 || oldStartingIndex >= view.Count || !Equals(view[oldStartingIndex], oldItem))
        {
            oldStartingIndex = view.IndexOf(oldItem);
        }

        if (oldStartingIndex < 0)
        {
            return;
        }

        RemoveFromView(oldStartingIndex, oldItem);
    }

    private void RemoveFromView(int itemIndex, object item)
    {
        view.RemoveAt(itemIndex);
        if (itemIndex <= CurrentPosition)
        {
            CurrentPosition--;
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
        if (i < -1 || i >= view.Count)
        {
            return false;
        }

        if (i == CurrentPosition)
        {
            return false;
        }

        CurrentChangingEventArgs e = new();
        OnCurrentChanging(e);
        if (e.Cancel)
        {
            return false;
        }

        CurrentPosition = i;
        OnCurrentChanged(default!);
        return true;
    }

    private void OnCurrentChanging(CurrentChangingEventArgs e)
    {
        if (deferCounter > 0)
        {
            return;
        }

        CurrentChanging?.Invoke(this, e);
    }

    private void OnCurrentChanged(object e)
    {
        if (deferCounter > 0)
        {
            return;
        }

        CurrentChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(CurrentItem));
    }

    private void OnVectorChanged(IVectorChangedEventArgs e)
    {
        if (deferCounter > 0)
        {
            return;
        }

        VectorChanged?.Invoke(this, e);
        OnPropertyChanged(nameof(Count));
    }

    internal sealed class NotificationDeferrer : IDisposable
    {
        private readonly AdvancedCollectionView advancedCollectionView;
        private readonly object? currentItem;

        public NotificationDeferrer(AdvancedCollectionView acvs)
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
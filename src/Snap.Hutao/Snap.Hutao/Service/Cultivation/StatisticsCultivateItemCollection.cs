// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.ViewModel.Cultivation;
using System.Collections;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Cultivation;

internal sealed partial class StatisticsCultivateItemCollection : ICollection<StatisticsCultivateItem>
{
    private readonly List<StatisticsCultivateItem> items = [];

    public StatisticsCultivateItemCollection(Dictionary<uint, StatisticsCultivateItem> items)
    {
        foreach ((_, StatisticsCultivateItem item) in items)
        {
            this.items.Add(item);
        }

        this.items.Sort(StatisticsCultivateItemComparer.Shared);
    }

    public int Count
    {
        get => items.Count;
    }

    public bool IsReadOnly
    {
        get => ((ICollection<StatisticsCultivateItem>)items).IsReadOnly;
    }

    public void SortAsIncompleteFirst()
    {
        // .OrderBy(item => item.IsFinished)
        // .ThenByDescending(item => item.IsToday)
        // .ThenBy(item => item.Inner.Id, MaterialIdComparer.Shared)
        items.Sort(static (x, y) =>
        {
            int result = x.IsFinished.CompareTo(y.IsFinished);
            if (result is not 0)
            {
                return result;
            }

            result = y.IsToday.CompareTo(x.IsToday);
            if (result is not 0)
            {
                return result;
            }

            return MaterialIdComparer.Shared.Compare(x.Inner.Id, y.Inner.Id);
        });
    }

    public ObservableCollection<StatisticsCultivateItem> ToObservableCollection()
    {
        List<StatisticsCultivateItem> results = [];
        foreach (StatisticsCultivateItem item in items)
        {
            if (item.ExcludedFromPresentation)
            {
                continue;
            }

            results.Add(item);
        }

        return results.ToObservableCollection();
    }

    public IEnumerator<StatisticsCultivateItem> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)items).GetEnumerator();
    }

    public void Add(StatisticsCultivateItem item)
    {
        items.Add(item);
    }

    public void Clear()
    {
        items.Clear();
    }

    public bool Contains(StatisticsCultivateItem item)
    {
        return items.Contains(item);
    }

    public void CopyTo(StatisticsCultivateItem[] array, int arrayIndex)
    {
        items.CopyTo(array, arrayIndex);
    }

    public bool Remove(StatisticsCultivateItem item)
    {
        return items.Remove(item);
    }
}
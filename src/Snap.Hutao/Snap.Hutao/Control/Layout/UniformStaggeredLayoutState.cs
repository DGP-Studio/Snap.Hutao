// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.Control.Layout;

internal sealed class UniformStaggeredLayoutState
{
    private readonly List<UniformStaggeredItem> items = new();
    private readonly VirtualizingLayoutContext context;
    private readonly Dictionary<int, UniformStaggeredColumnLayout> columnLayout = new();
    private double lastAverageHeight;

    public UniformStaggeredLayoutState(VirtualizingLayoutContext context)
    {
        this.context = context;
    }

    public double ColumnWidth { get; internal set; }

    public int NumberOfColumns
    {
        get => columnLayout.Count;
    }

    public double RowSpacing { get; internal set; }

    internal void AddItemToColumn(UniformStaggeredItem item, int columnIndex)
    {
        if (this.columnLayout.TryGetValue(columnIndex, out UniformStaggeredColumnLayout? columnLayout) == false)
        {
            columnLayout = new UniformStaggeredColumnLayout();
            this.columnLayout[columnIndex] = columnLayout;
        }

        if (columnLayout.Contains(item) == false)
        {
            columnLayout.Add(item);
        }
    }

    internal UniformStaggeredItem GetItemAt(int index)
    {
        if (index < 0)
        {
            throw new IndexOutOfRangeException();
        }

        if (index <= (items.Count - 1))
        {
            return items[index];
        }
        else
        {
            UniformStaggeredItem item = new(index);
            items.Add(item);
            return item;
        }
    }

    internal UniformStaggeredColumnLayout GetColumnLayout(int columnIndex)
    {
        this.columnLayout.TryGetValue(columnIndex, out UniformStaggeredColumnLayout? columnLayout);
        return columnLayout!;
    }

    /// <summary>
    /// Clear everything that has been calculated.
    /// </summary>
    internal void Clear()
    {
        columnLayout.Clear();
        items.Clear();
    }

    /// <summary>
    /// Clear the layout columns so they will be recalculated.
    /// </summary>
    internal void ClearColumns()
    {
        columnLayout.Clear();
    }

    /// <summary>
    /// Gets the estimated height of the layout.
    /// </summary>
    /// <returns>The estimated height of the layout.</returns>
    /// <remarks>
    /// If all of the items have been calculated then the actual height will be returned.
    /// If all of the items have not been calculated then an estimated height will be calculated based on the average height of the items.
    /// </remarks>
    internal double GetHeight()
    {
        double desiredHeight = Enumerable.Max(columnLayout.Values, c => c.Height);

        int itemCount = Enumerable.Sum(columnLayout.Values, c => c.Count);
        if (itemCount == context.ItemCount)
        {
            return desiredHeight;
        }

        double averageHeight = 0;
        foreach (KeyValuePair<int, UniformStaggeredColumnLayout> kvp in columnLayout)
        {
            averageHeight += kvp.Value.Height / kvp.Value.Count;
        }

        averageHeight /= columnLayout.Count;
        double estimatedHeight = (averageHeight * context.ItemCount) / columnLayout.Count;
        if (estimatedHeight > desiredHeight)
        {
            desiredHeight = estimatedHeight;
        }

        if (Math.Abs(desiredHeight - lastAverageHeight) < 5)
        {
            return lastAverageHeight;
        }

        lastAverageHeight = desiredHeight;
        return desiredHeight;
    }

    internal void RecycleElementAt(int index)
    {
        UIElement element = context.GetOrCreateElementAt(index);
        context.RecycleElement(element);
    }

    internal void RemoveFromIndex(int index)
    {
        if (index >= items.Count)
        {
            // Item was added/removed but we haven't realized that far yet
            return;
        }

        int numToRemove = items.Count - index;
        items.RemoveRange(index, numToRemove);

        foreach (KeyValuePair<int, UniformStaggeredColumnLayout> kvp in columnLayout)
        {
            UniformStaggeredColumnLayout layout = kvp.Value;
            for (int i = 0; i < layout.Count; i++)
            {
                if (layout[i].Index >= index)
                {
                    numToRemove = layout.Count - i;
                    layout.RemoveRange(i, numToRemove);
                    break;
                }
            }
        }
    }

    internal void RemoveRange(int startIndex, int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (i > items.Count)
            {
                break;
            }

            UniformStaggeredItem item = items[i];
            item.Height = 0;
            item.Top = 0;

            // We must recycle all elements to ensure that it gets the correct context
            RecycleElementAt(i);
        }

        foreach ((int key, UniformStaggeredColumnLayout layout) in columnLayout)
        {
            for (int i = 0; i < layout.Count; i++)
            {
                if ((startIndex <= layout[i].Index) && (layout[i].Index <= endIndex))
                {
                    int numToRemove = layout.Count - i;
                    layout.RemoveRange(i, numToRemove);
                    break;
                }
            }
        }
    }
}
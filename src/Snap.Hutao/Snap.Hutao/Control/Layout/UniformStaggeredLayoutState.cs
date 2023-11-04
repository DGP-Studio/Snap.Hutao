// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;

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
        if (!this.columnLayout.TryGetValue(columnIndex, out UniformStaggeredColumnLayout? columnLayout))
        {
            columnLayout = new();
            this.columnLayout[columnIndex] = columnLayout;
        }

        if (!columnLayout.Contains(item))
        {
            columnLayout.Add(item);
        }
    }

    [SuppressMessage("", "CA2201")]
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

    [SuppressMessage("", "SH007")]
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
        double desiredHeight = columnLayout.Values.Max(c => c.Height);
        int itemCount = columnLayout.Values.Sum(c => c.Count);

        if (itemCount == context.ItemCount)
        {
            return desiredHeight;
        }

        double averageHeight = 0;
        foreach ((_, UniformStaggeredColumnLayout layout) in columnLayout)
        {
            averageHeight += layout.Height / layout.Count;
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

        foreach ((_, UniformStaggeredColumnLayout layout) in columnLayout)
        {
            Span<UniformStaggeredItem> layoutSpan = CollectionsMarshal.AsSpan(layout);
            for (int i = 0; i < layoutSpan.Length; i++)
            {
                if (layoutSpan[i].Index >= index)
                {
                    numToRemove = layoutSpan.Length - i;
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

            ref readonly UniformStaggeredItem item = ref CollectionsMarshal.AsSpan(items)[i];
            item.Height = 0;
            item.Top = 0;

            // We must recycle all elements to ensure that it gets the correct context
            RecycleElementAt(i);
        }

        foreach ((_, UniformStaggeredColumnLayout layout) in columnLayout)
        {
            Span<UniformStaggeredItem> layoutSpan = CollectionsMarshal.AsSpan(layout);
            for (int i = 0; i < layoutSpan.Length; i++)
            {
                if ((startIndex <= layoutSpan[i].Index) && (layoutSpan[i].Index <= endIndex))
                {
                    int numToRemove = layoutSpan.Length - i;
                    layout.RemoveRange(i, numToRemove);
                    break;
                }
            }
        }
    }
}
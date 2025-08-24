// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.Layout;

internal sealed class UniformStaggeredLayoutState
{
    private readonly List<UniformStaggeredItem> items = [];
    private readonly VirtualizingLayoutContext context;
    private readonly Dictionary<int, UniformStaggeredColumnLayout> columnLayout = [];
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
            columnLayout = [];
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

        if (index <= items.Count - 1)
        {
            return items[index];
        }

        UniformStaggeredItem item = new(index);
        items.Add(item);
        return item;
    }

    internal UniformStaggeredColumnLayout GetColumnLayout(int columnIndex)
    {
        return columnLayout[columnIndex];
    }

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
        double estimatedHeight = averageHeight * context.ItemCount / columnLayout.Count;
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

    internal void Clear()
    {
        try
        {
            if (items.Count > 0)
            {
                RecycleElements();
            }
        }
        catch
        {
            // Ignore
        }

        ClearColumns();
        ClearItems();
    }

    internal void ClearColumns()
    {
        columnLayout.Clear();
    }

    internal void ClearItems()
    {
        items.Clear();
    }

    internal void RecycleElements()
    {
        for (int i = 0; i < context.ItemCount; i++)
        {
            RecycleElementAt(i);
        }
    }

    internal void RecycleElementAt(int index)
    {
        context.RecycleElement(context.GetOrCreateElementAt(index));
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
            if (i >= items.Count)
            {
                break;
            }

            UniformStaggeredItem item = items[i];
            item.Height = 0;
            item.Top = 0;

            // We must recycle all removed elements to ensure that it gets the correct context
            RecycleElementAt(i);
        }

        foreach ((_, UniformStaggeredColumnLayout layout) in columnLayout)
        {
            for (int i = 0; i < layout.Count; i++)
            {
                if (startIndex <= layout[i].Index && layout[i].Index <= endIndex)
                {
                    int numToRemove = layout.Count - i;
                    layout.RemoveRange(i, numToRemove);
                    break;
                }
            }
        }
    }
}
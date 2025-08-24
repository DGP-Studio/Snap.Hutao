// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Layout;

internal sealed class WrapLayoutState
{
    private readonly List<WrapItem> items = [];
    private readonly VirtualizingLayoutContext context;

    public WrapLayoutState(VirtualizingLayoutContext context)
    {
        this.context = context;
    }

    public Orientation Orientation { get; private set; }

    public Size Spacing { get; set; }

    public double AvailableWidth { get; set; }

    public WrapItem GetItemAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);

        if (index <= items.Count - 1)
        {
            return items[index];
        }

        WrapItem item = new(index);
        items.Add(item);
        return item;
    }

    public void Clear()
    {
        try
        {
            for (int i = 0; i < context.ItemCount; i++)
            {
                RecycleElementAt(i);
            }
        }
        catch
        {
            // Ignore
        }

        items.Clear();
    }

    public void RemoveFromIndex(int index)
    {
        if (index >= items.Count)
        {
            // Item was added/removed, but we haven't realized that far yet
            return;
        }

        int numToRemove = items.Count - index;
        items.RemoveRange(index, numToRemove);
    }

    public void ClearPositions()
    {
        foreach (WrapItem item in items)
        {
            item.Position = WrapItem.EmptyPosition;
        }
    }

    public double GetHeight()
    {
        if (items.Count is 0)
        {
            return 0;
        }

        Point? lastPosition = default;
        double maxHeight = 0;

        for (int i = items.Count - 1; i >= 0; --i)
        {
            WrapItem item = items[i];

            if (item.Position == WrapItem.EmptyPosition || item.Size == Size.Empty)
            {
                continue;
            }

            if (lastPosition is not null && lastPosition.Value.Y > item.Position.Y)
            {
                // This is a row above the last item.
                break;
            }

            lastPosition = item.Position;
            maxHeight = Math.Max(maxHeight, item.Size.Height);
        }

        return lastPosition?.Y + maxHeight ?? 0;
    }

    public void RecycleElementAt(int index)
    {
        context.RecycleElement(context.GetOrCreateElementAt(index));
    }
}
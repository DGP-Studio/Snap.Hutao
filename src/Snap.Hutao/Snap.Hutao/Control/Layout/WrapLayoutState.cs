// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.Control.Layout;

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

        if (index <= (items.Count - 1))
        {
            return items[index];
        }
        else
        {
            WrapItem item = new(index);
            items.Add(item);
            return item;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < items.Count; i++)
        {
            RecycleElementAt(i);
        }

        items.Clear();
    }

    public void RemoveFromIndex(int index)
    {
        if (index >= items.Count)
        {
            // Item was added/removed but we haven't realized that far yet
            return;
        }

        int numToRemove = items.Count - index;
        items.RemoveRange(index, numToRemove);
    }

    public void ClearPositions()
    {
        foreach (ref readonly WrapItem item in CollectionsMarshal.AsSpan(items))
        {
            item.Position = default;
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

            if (item.Position is null || item.Size is null)
            {
                continue;
            }

            if (lastPosition is not null && lastPosition.Value.Y > item.Position.Value.Y)
            {
                // This is a row above the last item.
                break;
            }

            lastPosition = item.Position;
            maxHeight = Math.Max(maxHeight, item.Size.Value.Height);
        }

        return lastPosition?.Y + maxHeight ?? 0;
    }

    public void RecycleElementAt(int index)
    {
        UIElement element = context.GetOrCreateElementAt(index);
        context.RecycleElement(element);
    }
}

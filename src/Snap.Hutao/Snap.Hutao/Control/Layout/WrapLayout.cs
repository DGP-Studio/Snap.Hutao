// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using Windows.Foundation;

namespace Snap.Hutao.Control.Layout;

[DependencyProperty("HorizontalSpacing", typeof(double), 0D, nameof(LayoutPropertyChanged))]
[DependencyProperty("VerticalSpacing", typeof(double), 0D, nameof(LayoutPropertyChanged))]
internal sealed partial class WrapLayout : VirtualizingLayout
{
    protected override void InitializeForContextCore(VirtualizingLayoutContext context)
    {
        context.LayoutState = new WrapLayoutState(context);
        base.InitializeForContextCore(context);
    }

    protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
    {
        context.LayoutState = default;
        base.UninitializeForContextCore(context);
    }

    protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
    {
        WrapLayoutState state = (WrapLayoutState)context.LayoutState;

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                state.RemoveFromIndex(args.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Move:
                int minIndex = Math.Min(args.NewStartingIndex, args.OldStartingIndex);
                state.RemoveFromIndex(minIndex);
                state.RecycleElementAt(args.OldStartingIndex);
                state.RecycleElementAt(args.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Remove:
                state.RemoveFromIndex(args.OldStartingIndex);
                break;

            case NotifyCollectionChangedAction.Replace:
                state.RemoveFromIndex(args.NewStartingIndex);
                state.RecycleElementAt(args.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Reset:
                state.Clear();
                break;
        }

        base.OnItemsChangedCore(context, source, args);
    }

    protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
    {
        Size spacing = new(HorizontalSpacing, VerticalSpacing);

        WrapLayoutState state = (WrapLayoutState)context.LayoutState;

        if (spacing != state.Spacing || state.AvailableWidth != availableSize.Width)
        {
            state.ClearPositions();
            state.Spacing = spacing;
            state.AvailableWidth = availableSize.Height;
        }

        double currentHeight = 0;
        Point position = default;
        for (int i = 0; i < context.ItemCount; ++i)
        {
            bool measured = false;
            WrapItem item = state.GetItemAt(i);
            if (item.Size is null)
            {
                item.Element = context.GetOrCreateElementAt(i);
                item.Element.Measure(availableSize);
                item.Size = item.Element.DesiredSize;
                measured = true;
            }

            Size currentSize = item.Size.Value;

            if (item.Position is null)
            {
                if (availableSize.Width < position.X + currentSize.Height)
                {
                    // New Row
                    position.X = 0;
                    position.Y += currentHeight + spacing.Height;
                    currentHeight = 0;
                }

                item.Position = position;
            }

            position = item.Position.Value;

            double vEnd = position.Y + currentSize.Width;
            if (vEnd < context.RealizationRect.Top)
            {
                // Item is "above" the bounds
                if (item.Element is not null)
                {
                    context.RecycleElement(item.Element);
                    item.Element = default;
                }

                continue;
            }
            else if (position.Y > context.RealizationRect.Bottom)
            {
                // Item is "below" the bounds.
                if (item.Element is not null)
                {
                    context.RecycleElement(item.Element);
                    item.Element = default;
                }

                // We don't need to measure anything below the bounds
                break;
            }
            else if (!measured)
            {
                // Always measure elements that are within the bounds
                item.Element = context.GetOrCreateElementAt(i);
                item.Element.Measure(availableSize);

                currentSize = item.Element.DesiredSize;
                if (currentSize != item.Size)
                {
                    // this item changed size; we need to recalculate layout for everything after this
                    state.RemoveFromIndex(i + 1);
                    item.Size = currentSize;

                    // did the change make it go into the new row?
                    if (availableSize.Width < position.X + currentSize.Width)
                    {
                        // New Row
                        position.X = 0;
                        position.Y += currentHeight + spacing.Height;
                        currentHeight = 0;
                    }

                    item.Position = position;
                }
            }

            position.X += currentSize.Width + spacing.Width;
            currentHeight = Math.Max(currentSize.Height, currentHeight);
        }

        return new Size(double.IsInfinity(availableSize.Width) ? 0 : Math.Ceiling(availableSize.Width), state.GetHeight());
    }

    protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
    {
        if (context.ItemCount > 0)
        {
            WrapLayoutState state = (WrapLayoutState)context.LayoutState;

            bool ArrangeItem(WrapItem item)
            {
                if (item is { Size: null } or { Position: null })
                {
                    return false;
                }

                Size desiredSize = item.Size.Value;

                Point position = item.Position.Value;

                if (context.RealizationRect.Top <= position.Y + desiredSize.Height && position.Y <= context.RealizationRect.Bottom)
                {
                    // place the item
                    UIElement child = context.GetOrCreateElementAt(item.Index);
                    child.Arrange(new Rect(position, desiredSize));
                }
                else if (position.Y > context.RealizationRect.Bottom)
                {
                    return false;
                }

                return true;
            }

            for (int i = 0; i < context.ItemCount; ++i)
            {
                if (!ArrangeItem(state.GetItemAt(i)))
                {
                    break;
                }
            }
        }

        return finalSize;
    }

    private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WrapLayout wp)
        {
            wp.InvalidateMeasure();
            wp.InvalidateArrange();
        }
    }
}

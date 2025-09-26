// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using Windows.Foundation;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control.Layout;

[DependencyProperty<double>("HorizontalSpacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(LayoutPropertyChanged), NotNull = true)]
[DependencyProperty<double>("VerticalSpacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(LayoutPropertyChanged), NotNull = true)]
internal sealed partial class WrapLayout : VirtualizingLayout
{
    protected override void InitializeForContextCore(VirtualizingLayoutContext context)
    {
        context.LayoutState = new WrapLayoutState(context);
    }

    protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
    {
        context.LayoutState = default;
    }

    protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
    {
        WrapLayoutState state = context.LayoutState.As<WrapLayoutState>();

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
    }

    protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
    {
        if (context.ItemCount is 0 || context.RealizationRect is { Width: 0, Height: 0 })
        {
            return new Size(availableSize.Width, 0);
        }

        Size spacing = new(HorizontalSpacing, VerticalSpacing);

        WrapLayoutState state = context.LayoutState.As<WrapLayoutState>();

        if (spacing != state.Spacing || state.AvailableWidth != availableSize.Width)
        {
            state.ClearPositions();
            state.Spacing = spacing;
            state.AvailableWidth = availableSize.Width;
        }

        double currentHeight = 0;
        Point itemPosition = default;
        for (int i = 0; i < context.ItemCount; ++i)
        {
            bool itemMeasured = false;
            WrapItem item = state.GetItemAt(i);
            if (item.Size == Size.Empty)
            {
                item.Element = context.GetOrCreateElementAt(i);
                item.Element.Measure(availableSize);
                item.Size = item.Element.DesiredSize;
                itemMeasured = true;
            }

            Size itemSize = item.Size;

            if (item.Position == WrapItem.EmptyPosition)
            {
                if (availableSize.Width < itemPosition.X + itemSize.Width)
                {
                    // New Row
                    itemPosition.X = 0;
                    itemPosition.Y += currentHeight + spacing.Height;
                    currentHeight = 0;
                }

                item.Position = itemPosition;
            }

            itemPosition = item.Position;

            double bottom = itemPosition.Y + itemSize.Height;
            if (bottom < context.RealizationRect.Top)
            {
                // Item is "above" the bounds
                if (item.Element is not null)
                {
                    context.RecycleElement(item.Element);
                    item.Element = default;
                }

                continue;
            }

            if (itemPosition.Y > context.RealizationRect.Bottom)
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

            if (!itemMeasured)
            {
                // Always measure elements that are within the bounds
                item.Element = context.GetOrCreateElementAt(i);
                item.Element.Measure(availableSize);

                itemSize = item.Element.DesiredSize;
                if (itemSize != item.Size)
                {
                    // this item changed size; we need to recalculate layout for everything after this
                    state.RemoveFromIndex(i + 1);
                    item.Size = itemSize;

                    // did the change make it go into the new row?
                    if (availableSize.Width < itemPosition.X + itemSize.Width)
                    {
                        // New Row
                        itemPosition.X = 0;
                        itemPosition.Y += currentHeight + spacing.Height;
                        currentHeight = 0;
                    }

                    item.Position = itemPosition;
                }
            }

            itemPosition.X += itemSize.Width + spacing.Width;
            currentHeight = Math.Max(itemSize.Height, currentHeight);
        }

        return new Size(double.IsInfinity(availableSize.Width) ? 0 : Math.Ceiling(availableSize.Width), state.GetHeight());
    }

    protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
    {
        if (context.ItemCount > 0)
        {
            WrapLayoutState state = context.LayoutState.As<WrapLayoutState>();

            for (int i = 0; i < context.ItemCount; ++i)
            {
                if (!ArrangeItem(context, state.GetItemAt(i)))
                {
                    break;
                }
            }
        }

        return finalSize;

        static bool ArrangeItem(VirtualizingLayoutContext context, WrapItem item)
        {
            if (item.Size == Size.Empty || item.Position == WrapItem.EmptyPosition)
            {
                return false;
            }

            Size size = item.Size;
            Point position = item.Position;

            if (context.RealizationRect.Top <= position.Y + size.Height && position.Y <= context.RealizationRect.Bottom)
            {
                // place the item
                UIElement child = context.GetOrCreateElementAt(item.Index);
                child.Arrange(new Rect(position, size));
            }
            else if (position.Y > context.RealizationRect.Bottom)
            {
                return false;
            }

            return true;
        }
    }

    private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WrapLayout layout)
        {
            layout.InvalidateMeasure();
            layout.InvalidateArrange();
        }
    }
}
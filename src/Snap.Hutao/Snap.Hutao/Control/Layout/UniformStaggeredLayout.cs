// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.Control.Layout;

[DependencyProperty("MinItemWidth", typeof(double), 0D, nameof(OnMinItemWidthChanged))]
[DependencyProperty("MinColumnSpacing", typeof(double), 0D, nameof(OnSpacingChanged))]
[DependencyProperty("MinRowSpacing", typeof(double), 0D, nameof(OnSpacingChanged))]
internal sealed partial class UniformStaggeredLayout : VirtualizingLayout
{
    /// <inheritdoc/>
    protected override void InitializeForContextCore(VirtualizingLayoutContext context)
    {
        context.LayoutState = new UniformStaggeredLayoutState(context);
        base.InitializeForContextCore(context);
    }

    /// <inheritdoc/>
    protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
    {
        context.LayoutState = null;
        base.UninitializeForContextCore(context);
    }

    /// <inheritdoc/>
    protected override void OnItemsChangedCore(VirtualizingLayoutContext context, object source, NotifyCollectionChangedEventArgs args)
    {
        UniformStaggeredLayoutState state = (UniformStaggeredLayoutState)context.LayoutState;

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                state.RemoveFromIndex(args.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Replace:
                state.RemoveFromIndex(args.NewStartingIndex);
                state.RecycleElementAt(args.NewStartingIndex); // We must recycle the element to ensure that it gets the correct context
                break;

            case NotifyCollectionChangedAction.Move:
                int minIndex = Math.Min(args.NewStartingIndex, args.OldStartingIndex);
                int maxIndex = Math.Max(args.NewStartingIndex, args.OldStartingIndex);
                state.RemoveRange(minIndex, maxIndex);
                break;

            case NotifyCollectionChangedAction.Remove:
                state.RemoveFromIndex(args.OldStartingIndex);
                break;

            case NotifyCollectionChangedAction.Reset:
                state.Clear();
                break;
        }

        base.OnItemsChangedCore(context, source, args);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
    {
        if (context.ItemCount == 0)
        {
            return new Size(availableSize.Width, 0);
        }

        if ((context.RealizationRect.Width == 0) && (context.RealizationRect.Height == 0))
        {
            return new Size(availableSize.Width, 0.0f);
        }

        UniformStaggeredLayoutState state = (UniformStaggeredLayoutState)context.LayoutState;

        double availableWidth = availableSize.Width;
        double availableHeight = availableSize.Height;

        (int numberOfColumns, double columnWidth) = GetNumberOfColumnsAndWidth(availableWidth, MinItemWidth, MinColumnSpacing);

        if (columnWidth != state.ColumnWidth)
        {
            // The items will need to be remeasured
            state.Clear();
        }

        state.ColumnWidth = columnWidth;

        // adjust for column spacing on all columns expect the first
        double totalWidth = state.ColumnWidth + ((numberOfColumns - 1) * (state.ColumnWidth + MinColumnSpacing));
        if (totalWidth > availableWidth)
        {
            numberOfColumns--;
        }
        else if (double.IsInfinity(availableWidth))
        {
            availableWidth = totalWidth;
        }

        if (numberOfColumns != state.NumberOfColumns)
        {
            // The items will not need to be remeasured, but they will need to go into new columns
            state.ClearColumns();
        }

        if (MinRowSpacing != state.RowSpacing)
        {
            // If the RowSpacing changes the height of the rows will be different.
            // The columns stores the height so we'll want to clear them out to
            // get the proper height
            state.ClearColumns();
            state.RowSpacing = MinRowSpacing;
        }

        Span<double> columnHeights = new double[numberOfColumns];
        Span<int> itemsPerColumn = new int[numberOfColumns];
        HashSet<int> deadColumns = new();

        for (int i = 0; i < context.ItemCount; i++)
        {
            int columnIndex = GetLowestColumnIndex(columnHeights);

            bool measured = false;
            UniformStaggeredItem item = state.GetItemAt(i);
            if (item.Height == 0)
            {
                // Item has not been measured yet. Get the element and store the values
                UIElement element = context.GetOrCreateElementAt(i);
                element.Measure(new Size(state.ColumnWidth, availableHeight));
                item.Height = element.DesiredSize.Height;
                item.Element = element;
                measured = true;
            }

            double spacing = itemsPerColumn[columnIndex] > 0 ? MinRowSpacing : 0;
            item.Top = columnHeights[columnIndex] + spacing;
            double bottom = item.Top + item.Height;
            columnHeights[columnIndex] = bottom;
            itemsPerColumn[columnIndex]++;
            state.AddItemToColumn(item, columnIndex);

            if (bottom < context.RealizationRect.Top)
            {
                // The bottom of the element is above the realization area
                if (item.Element is not null)
                {
                    context.RecycleElement(item.Element);
                    item.Element = null;
                }
            }
            else if (item.Top > context.RealizationRect.Bottom)
            {
                // The top of the element is below the realization area
                if (item.Element is not null)
                {
                    context.RecycleElement(item.Element);
                    item.Element = null;
                }

                deadColumns.Add(columnIndex);
            }
            else if (measured == false)
            {
                // We ALWAYS want to measure an item that will be in the bounds
                item.Element = context.GetOrCreateElementAt(i);
                item.Element.Measure(new Size(state.ColumnWidth, availableHeight));
                if (item.Height != item.Element.DesiredSize.Height)
                {
                    // this item changed size; we need to recalculate layout for everything after this
                    state.RemoveFromIndex(i + 1);
                    item.Height = item.Element.DesiredSize.Height;
                    columnHeights[columnIndex] = item.Top + item.Height;
                }
            }

            if (deadColumns.Count == numberOfColumns)
            {
                break;
            }
        }

        double desiredHeight = state.GetHeight();

        return new Size(availableWidth, desiredHeight);
    }

    /// <inheritdoc/>
    protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
    {
        if ((context.RealizationRect.Width == 0) && (context.RealizationRect.Height == 0))
        {
            return finalSize;
        }

        UniformStaggeredLayoutState state = (UniformStaggeredLayoutState)context.LayoutState;

        // Cycle through each column and arrange the items that are within the realization bounds
        for (int columnIndex = 0; columnIndex < state.NumberOfColumns; columnIndex++)
        {
            UniformStaggeredColumnLayout layout = state.GetColumnLayout(columnIndex);
            Span<UniformStaggeredItem> layoutSpan = CollectionsMarshal.AsSpan(layout);
            for (int i = 0; i < layoutSpan.Length; i++)
            {
                ref readonly UniformStaggeredItem item = ref layoutSpan[i];

                double bottom = item.Top + item.Height;
                if (bottom < context.RealizationRect.Top)
                {
                    // element is above the realization bounds
                    continue;
                }

                if (item.Top <= context.RealizationRect.Bottom)
                {
                    double itemHorizontalOffset = (state.ColumnWidth * columnIndex) + (MinColumnSpacing * columnIndex);

                    Rect bounds = new(itemHorizontalOffset, item.Top, state.ColumnWidth, item.Height);
                    UIElement element = context.GetOrCreateElementAt(item.Index);
                    element.Arrange(bounds);
                }
                else
                {
                    break;
                }
            }
        }

        return finalSize;
    }

    private static (int NumberOfColumns, double ColumnWidth) GetNumberOfColumnsAndWidth(double availableWidth, double minItemWidth, double minColumnSpacing)
    {
        // test if the width can fit in 2 items
        if ((2 * minItemWidth) + minColumnSpacing > availableWidth)
        {
            return (1, availableWidth);
        }

        int columnCount = Math.Max(1, (int)((availableWidth + minColumnSpacing) / (minItemWidth + minColumnSpacing)));
        double columnWidthAddSpacing = (availableWidth + minColumnSpacing) / columnCount;
        return (columnCount, columnWidthAddSpacing - minColumnSpacing);
    }

    private static int GetLowestColumnIndex(in ReadOnlySpan<double> columnHeights)
    {
        int columnIndex = 0;
        double height = columnHeights[0];
        for (int j = 1; j < columnHeights.Length; j++)
        {
            if (columnHeights[j] < height)
            {
                columnIndex = j;
                height = columnHeights[j];
            }
        }

        return columnIndex;
    }

    private static void OnMinItemWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UniformStaggeredLayout panel = (UniformStaggeredLayout)d;
        panel.InvalidateMeasure();
    }

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        UniformStaggeredLayout panel = (UniformStaggeredLayout)d;
        panel.InvalidateMeasure();
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty<double>("HorizontalSpacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnPropertyChanged), NotNull = true)]
[DependencyProperty<double>("VerticalSpacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnPropertyChanged), NotNull = true)]
[DependencyProperty<Thickness>("Padding", PropertyChangedCallbackName = nameof(OnPropertyChanged), NotNull = true)]
internal sealed partial class WrapPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private readonly List<Row> rows = [];

    protected override Size MeasureOverride(Size availableSize)
    {
        Size childAvailableSize = new(availableSize.Width - Padding.Left - Padding.Right, availableSize.Height - Padding.Top - Padding.Bottom);

        foreach (UIElement child in Children)
        {
            child.Measure(childAvailableSize);
        }

        return UpdateRows(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (finalSize.Width < DesiredSize.Width)
        {
            // We haven't received our desired size. We need to refresh the rows.
            UpdateRows(finalSize);
        }

        if (rows.Count > 0)
        {
            // Now that we have all the data, we do the actual arrange pass
            int childIndex = 0;
            foreach (Row row in rows)
            {
                foreach (Rect rect in row.ChildrenRects)
                {
                    UIElement child = Children[childIndex++];
                    while (child.Visibility is Visibility.Collapsed)
                    {
                        // Collapsed children are not added into the rows,
                        // we skip them.
                        child = Children[childIndex++];
                    }

                    Rect finalRect = new(rect.X, rect.Y, rect.Width, rect.Height);

                    child.Arrange(finalRect);
                }
            }
        }

        return finalSize;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WrapPanel wp)
        {
            wp.InvalidateMeasure();
            wp.InvalidateArrange();
        }
    }

    private Size UpdateRows(Size availableSize)
    {
        rows.Clear();

        Thickness padding = Padding;

        if (Children.Count is 0)
        {
            return new(padding.Left + padding.Right, padding.Top + padding.Bottom);
        }

        Size parentMeasure = new(availableSize.Width, availableSize.Height);
        Size spacingMeasure = new(HorizontalSpacing, VerticalSpacing);
        Point position = new(Padding.Left, Padding.Top);

        Row currentRow = new([], default);
        Size finalMeasure = default;

        foreach (UIElement child in Children)
        {
            if (child.Visibility is Visibility.Collapsed)
            {
                // if an item is collapsed, avoid adding the spacing
                continue;
            }

            Size desiredMeasure = child.DesiredSize;
            if (desiredMeasure.Width + position.X + padding.Right > parentMeasure.Width)
            {
                // next row!
                position.X = padding.Left;
                position.Y += currentRow.Size.Height + spacingMeasure.Height;

                rows.Add(currentRow);
                currentRow = new([], default);
            }

            currentRow.Add(position, desiredMeasure);

            // adjust the location for the next items
            position.X += desiredMeasure.Width + spacingMeasure.Width;
            finalMeasure.Width = Math.Max(finalMeasure.Width, position.X);
        }

        if (currentRow.ChildrenRects.Count > 0)
        {
            rows.Add(currentRow);
        }

        if (rows.Count is 0)
        {
            return new(padding.Left + padding.Right, padding.Top + padding.Bottom);
        }

        // Get max V here before computing final rect
        Rect lastRowRect = rows.Last().Rect;
        finalMeasure.Height = lastRowRect.Y + lastRowRect.Height;
        return new(finalMeasure.Width + padding.Right - spacingMeasure.Width, finalMeasure.Height + padding.Bottom);
    }

    private struct Row
    {
        public Row(List<Rect> childrenRects, Size size)
        {
            ChildrenRects = childrenRects;
            Size = size;
        }

        public List<Rect> ChildrenRects { get; }

        public Size Size { get; set; }

        public readonly Rect Rect
        {
            get => ChildrenRects.Count > 0
                ? new(ChildrenRects[0].X, ChildrenRects[0].Y, Size.Width, Size.Height)
                : new(0D, 0D, Size.Width, Size.Height);
        }

        public void Add(Point position, Size size)
        {
            ChildrenRects.Add(new(position.X, position.Y, size.Width, size.Height));
            Size = new(position.X + size.Width, Math.Max(Size.Height, size.Height));
        }
    }
}
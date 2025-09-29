// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty<double>("MinItemWidth", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnPropertyChanged), NotNull = true)]
[DependencyProperty<double>("ColumnSpacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnPropertyChanged), NotNull = true)]
[DependencyProperty<double>("RowSpacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnPropertyChanged), NotNull = true)]
internal sealed partial class UniformStaggeredPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private readonly List<Column> columns = [];

    protected override Size MeasureOverride(Size availableSize)
    {
        (int numberOfColumns, double columnWidth) = GetNumberOfColumnsAndWidth(availableSize.Width, MinItemWidth, ColumnSpacing);

        foreach (UIElement child in Children)
        {
            child.Measure(new(columnWidth, availableSize.Height));
        }

        return UpdateColumns(numberOfColumns, columnWidth);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (finalSize.Width < DesiredSize.Width)
        {
            (int numberOfColumns, double columnWidth) = GetNumberOfColumnsAndWidth(finalSize.Width, MinItemWidth, ColumnSpacing);
            UpdateColumns(numberOfColumns, columnWidth);
        }

        if (columns.Count > 0)
        {
            foreach (Column column in columns)
            {
                foreach ((int childIndex, Rect rect) in column.ChildrenRectMap)
                {
                    UIElement child = Children[childIndex];
                    Rect finalRect = rect;
                    finalRect.Width = column.Size.Width;
                    child.Arrange(finalRect);
                }
            }
        }

        return finalSize;
    }

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        d.As<UniformStaggeredPanel>().InvalidateMeasure();
    }

    private static (int ColumnCount, double ColumnWidth) GetNumberOfColumnsAndWidth(double availableWidth, double minItemWidth, double columnSpacing)
    {
        int columnCount = Math.Max(1, (int)((availableWidth + columnSpacing) / (minItemWidth + columnSpacing)));
        double columnWidth = ((availableWidth + columnSpacing) / columnCount) - columnSpacing;
        return (columnCount, columnWidth);
    }

    private Size UpdateColumns(int columnCount, double columnWidth)
    {
        columns.Clear();

        if (Children.Count is 0)
        {
            return default;
        }

        for (int i = 0; i < columnCount; i++)
        {
            columns.Add(new(i, columnWidth, ColumnSpacing));
        }

        foreach ((int index, UIElement child) in Children.Index())
        {
            if (child.Visibility is Visibility.Collapsed)
            {
                continue;
            }

            Column? currentColumn = columns.MinBy(c => c.Size.Height);
            ArgumentNullException.ThrowIfNull(currentColumn);
            Point position = new(currentColumn.StartX, currentColumn.Size.Height);
            if (currentColumn.ChildrenRectMap.Count > 0)
            {
                position.Y += RowSpacing;
            }

            currentColumn.Add(index, position, child.DesiredSize);
        }

        if (columns.Count is 0)
        {
            return default;
        }

        Column? maxHeightColumn = columns.MaxBy(c => c.Size.Height);
        ArgumentNullException.ThrowIfNull(maxHeightColumn);

        return new((columnCount * (columnWidth + ColumnSpacing)) - ColumnSpacing, maxHeightColumn.Size.Height);
    }

    private sealed class Column
    {
        public Column(int index, double width, double spacing)
        {
            Size = new(width, 0);
            StartX = index * (width + spacing);
        }

        public double StartX { get; }

        public Dictionary<int, Rect> ChildrenRectMap { get; } = [];

        public Size Size { get; private set; }

        public void Add(int childIndex, Point position, Size size)
        {
            ChildrenRectMap.Add(childIndex, new(position.X, position.Y, size.Width, size.Height));
            Size = new(Math.Max(Size.Width, size.Width), position.Y + size.Height);
        }
    }
}
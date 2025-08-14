// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty<double>("MinItemWidth", NotNull = true)]
[DependencyProperty<double>("ColumnSpacing", NotNull = true)]
[DependencyProperty<double>("RowSpacing", NotNull = true)]
internal sealed partial class UniformPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private int columnCount;

    protected override Size MeasureOverride(Size availableSize)
    {
        // https://hut.ao/tasks/86
        // Handle when MinItemWith > availableSize.Width
        columnCount = Math.Max(1, (int)((availableSize.Width + ColumnSpacing) / (MinItemWidth + ColumnSpacing)));
        double availableItemWidth = ((availableSize.Width + ColumnSpacing) / columnCount) - ColumnSpacing;

        double maxDesiredHeight = 0;
        foreach (UIElement child in Children)
        {
            child.Measure(new(availableItemWidth, availableSize.Height));
            maxDesiredHeight = Math.Max(maxDesiredHeight, child.DesiredSize.Height);
        }

        int desiredRows = (int)Math.Ceiling(Children.Count / (double)columnCount);
        double desiredHeight = ((maxDesiredHeight + RowSpacing) * desiredRows) - RowSpacing;
        desiredHeight = Math.Max(0, desiredHeight);

        return new(Math.Max(0, availableSize.Width), desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double maxItemHeight = 0;
        foreach (UIElement child in Children)
        {
            if (child.DesiredSize.Height > maxItemHeight)
            {
                maxItemHeight = child.DesiredSize.Height;
            }
        }

        double itemWidth = ((finalSize.Width + ColumnSpacing) / columnCount) - ColumnSpacing;

        for (int index = 0; index < Children.Count; index++)
        {
            UIElement child = Children[index];

            int row = index / columnCount;
            int column = index % columnCount;

            double x = column * (itemWidth + ColumnSpacing);
            double y = row * (maxItemHeight + RowSpacing);

            child.Arrange(new(x, y, itemWidth, child.DesiredSize.Height));
        }

        return finalSize;
    }
}
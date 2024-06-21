// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty("MinItemWidth", typeof(double))]
[DependencyProperty("ColumnSpacing", typeof(double))]
[DependencyProperty("RowSpacing", typeof(double))]
internal sealed partial class UniformPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private int columns;

    protected override Size MeasureOverride(Size availableSize)
    {
        columns = (int)((availableSize.Width + ColumnSpacing) / (MinItemWidth + ColumnSpacing));
        double availableItemWidth = ((availableSize.Width + ColumnSpacing) / columns) - ColumnSpacing;

        double maxDesiredHeight = 0;
        foreach (UIElement child in Children)
        {
            child.Measure(new Size(availableItemWidth, availableSize.Height));
            maxDesiredHeight = Math.Max(maxDesiredHeight, child.DesiredSize.Height);
        }

        int desiredRows = (int)Math.Ceiling(Children.Count / (double)columns);
        double desiredHeight = ((maxDesiredHeight + RowSpacing) * desiredRows) - RowSpacing;

        return new Size(availableSize.Width, desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double itemWidth = ((finalSize.Width + ColumnSpacing) / columns) - ColumnSpacing;

        for (int index = 0; index < Children.Count; index++)
        {
            UIElement child = Children[index];

            int row = index / columns;
            int column = index % columns;

            double x = column * (itemWidth + ColumnSpacing);
            double y = row * (child.DesiredSize.Height + RowSpacing);

            child.Arrange(new Rect(x, y, itemWidth, child.DesiredSize.Height));
        }

        return finalSize;
    }
}
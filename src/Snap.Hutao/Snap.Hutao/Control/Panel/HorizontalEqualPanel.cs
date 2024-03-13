// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

[DependencyProperty("MinItemWidth", typeof(double))]
[DependencyProperty("Spacing", typeof(double))]
internal partial class HorizontalEqualPanel : Microsoft.UI.Xaml.Controls.Panel
{
    public HorizontalEqualPanel()
    {
        Loaded += OnLoaded;
        SizeChanged += OnSizeChanged;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (UIElement child in Children)
        {
            // ScrollViewer will always return an Infinity Size, we should use ActualWidth for this situation.
            double availableWidth = double.IsInfinity(availableSize.Width) ? ActualWidth : availableSize.Width;
            double childAvailableWidth = (availableWidth + Spacing) / Children.Count;
            double childMaxAvailableWidth = Math.Max(MinItemWidth, childAvailableWidth);
            child.Measure(new(childMaxAvailableWidth - Spacing, ActualHeight));
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int itemCount = Children.Count;
        double availableWidthPerItem = (finalSize.Width - (Spacing * (itemCount - 1))) / itemCount;
        double actualItemWidth = Math.Max(MinItemWidth, availableWidthPerItem);

        double offset = 0;
        foreach (UIElement child in Children)
        {
            child.Arrange(new Rect(offset, 0, actualItemWidth, finalSize.Height));
            offset += actualItemWidth + Spacing;
        }

        return finalSize;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MinWidth = (MinItemWidth * Children.Count) + (Spacing * (Children.Count - 1));
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        InvalidateMeasure();
    }
}
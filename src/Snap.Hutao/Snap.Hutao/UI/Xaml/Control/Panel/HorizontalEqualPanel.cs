// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

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
        List<UIElement> visibleChildren = Children.Where(child => child.Visibility is Visibility.Visible).ToList();
        foreach (ref readonly UIElement visibleChild in CollectionsMarshal.AsSpan(visibleChildren))
        {
            // ScrollViewer will always return an Infinity Size, we should use ActualWidth for this situation.
            double availableWidth = double.IsInfinity(availableSize.Width) ? ActualWidth : availableSize.Width;
            double childAvailableWidth = (availableWidth + Spacing) / visibleChildren.Count;
            double childMaxAvailableWidth = Math.Max(MinItemWidth, childAvailableWidth);
            visibleChild.Measure(new(childMaxAvailableWidth - Spacing, ActualHeight));
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        List<UIElement> visibleChildren = Children.Where(child => child.Visibility is Visibility.Visible).ToList();
        double availableItemWidth = (finalSize.Width - (Spacing * (visibleChildren.Count - 1))) / visibleChildren.Count;
        double actualItemWidth = Math.Max(MinItemWidth, availableItemWidth);

        double offset = 0;
        foreach (ref readonly UIElement visibleChild in CollectionsMarshal.AsSpan(visibleChildren))
        {
            visibleChild.Arrange(new Rect(offset, 0, actualItemWidth, finalSize.Height));
            offset += actualItemWidth + Spacing;
        }

        return finalSize;
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        HorizontalEqualPanel panel = (HorizontalEqualPanel)sender;
        int vivibleChildrenCount = panel.Children.Count(child => child.Visibility is Visibility.Visible);
        panel.MinWidth = (panel.MinItemWidth * vivibleChildrenCount) + (panel.Spacing * (vivibleChildrenCount - 1));
    }

    private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ((HorizontalEqualPanel)sender).InvalidateMeasure();
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty("Spacing", typeof(double), default(double), nameof(OnSpacingChanged))]
internal partial class EqualPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private double maxItemWidth;
    private double maxItemHeight;
    private int visibleItemsCount;

    public EqualPanel()
    {
        RegisterPropertyChangedCallback(HorizontalAlignmentProperty, OnHorizontalAlignmentChanged);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        maxItemWidth = 0;
        maxItemHeight = 0;

        List<UIElement> elements = Children.Where(element => element.Visibility is Visibility.Visible).ToList();
        visibleItemsCount = elements.Count;

        if (visibleItemsCount <= 0)
        {
            return default;
        }

        if (HorizontalAlignment is not HorizontalAlignment.Stretch || double.IsInfinity(availableSize.Width))
        {
            foreach (ref readonly UIElement child in CollectionsMarshal.AsSpan(elements))
            {
                child.Measure(availableSize);
                maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
                maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
            }

            return new((maxItemWidth * visibleItemsCount) + (Spacing * (visibleItemsCount - 1)), maxItemHeight);
        }

        double totalWidth = availableSize.Width - (Spacing * (visibleItemsCount - 1));
        maxItemWidth = totalWidth / visibleItemsCount;
        foreach (ref readonly UIElement child in CollectionsMarshal.AsSpan(elements))
        {
            child.Measure(new(maxItemWidth, availableSize.Height));
            maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
        }

        return new(availableSize.Width, maxItemHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double x = 0;

        // Check if there's more (little) width available - if so, set max item width to the maximum possible as we have an almost perfect height.
        if (finalSize.Width > (visibleItemsCount * maxItemWidth) + (Spacing * (visibleItemsCount - 1)))
        {
            maxItemWidth = (finalSize.Width - (Spacing * (visibleItemsCount - 1))) / visibleItemsCount;
        }

        foreach (UIElement child in Children.Where(e => e.Visibility is Visibility.Visible))
        {
            child.Arrange(new Rect(x, 0, maxItemWidth, maxItemHeight));
            x += maxItemWidth + Spacing;
        }

        return finalSize;
    }

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        (d as EqualPanel)?.InvalidateMeasure();
    }

    private static void OnHorizontalAlignmentChanged(DependencyObject d, DependencyProperty dp)
    {
        (d as EqualPanel)?.InvalidateMeasure();
    }
}
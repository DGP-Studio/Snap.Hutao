// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Data;
using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

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

        IEnumerable<UIElement> elements = Children.Where(static e => e.Visibility == Visibility.Visible);
        visibleItemsCount = elements.Count();

        foreach (UIElement child in elements)
        {
            child.Measure(availableSize);
            maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
            maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
        }

        if (visibleItemsCount > 0)
        {
            // Return equal widths based on the widest item
            // In very specific edge cases the AvailableWidth might be infinite resulting in a crash.
            if (HorizontalAlignment != HorizontalAlignment.Stretch || double.IsInfinity(availableSize.Width))
            {
                return new Size((maxItemWidth * visibleItemsCount) + (Spacing * (visibleItemsCount - 1)), maxItemHeight);
            }
            else
            {
                // Equal columns based on the available width, adjust for spacing
                double totalWidth = availableSize.Width - (Spacing * (visibleItemsCount - 1));
                maxItemWidth = totalWidth / visibleItemsCount;
                return new Size(availableSize.Width, maxItemHeight);
            }
        }
        else
        {
            return new Size(0, 0);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double x = 0;

        // Check if there's more (little) width available - if so, set max item width to the maximum possible as we have an almost perfect height.
        if (finalSize.Width > (visibleItemsCount * maxItemWidth) + (Spacing * (visibleItemsCount - 1)))
        {
            maxItemWidth = (finalSize.Width - (Spacing * (visibleItemsCount - 1))) / visibleItemsCount;
        }

        IEnumerable<UIElement> elements = Children.Where(static e => e.Visibility == Visibility.Visible);
        foreach (UIElement child in elements)
        {
            child.Arrange(new Rect(x, 0, maxItemWidth, maxItemHeight));
            x += maxItemWidth + Spacing;
        }

        return finalSize;
    }

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        EqualPanel panel = (EqualPanel)d;
        panel.InvalidateMeasure();
    }

    private void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyProperty dp)
    {
        InvalidateMeasure();
    }
}
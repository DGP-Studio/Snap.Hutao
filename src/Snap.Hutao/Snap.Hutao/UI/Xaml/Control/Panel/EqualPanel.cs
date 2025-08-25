// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty<double>("Spacing", DefaultValue = 0D, PropertyChangedCallbackName = nameof(OnSpacingChanged), NotNull = true)]
internal partial class EqualPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private readonly long horizontalAlignmentChangedToken;

    private double maxItemWidth;
    private double maxItemHeight;
    private int visibleItemsCount;

    public EqualPanel()
    {
        horizontalAlignmentChangedToken = RegisterPropertyChangedCallback(HorizontalAlignmentProperty, OnHorizontalAlignmentChanged);
        Unloaded += OnUnloaded;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        maxItemWidth = 0;
        maxItemHeight = 0;

        ReadOnlySpan<UIElement> visibleItems = [.. Children.Where(element => element.Visibility is Visibility.Visible)];
        visibleItemsCount = visibleItems.Length;

        if (visibleItemsCount <= 0)
        {
            return default;
        }

        if (HorizontalAlignment is not HorizontalAlignment.Stretch || double.IsInfinity(availableSize.Width))
        {
            foreach (ref readonly UIElement child in visibleItems)
            {
                child.Measure(availableSize);
                maxItemWidth = Math.Max(maxItemWidth, child.DesiredSize.Width);
                maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
            }

            return new(EqualPanelAlgorithm.GetTotalLength(maxItemWidth, visibleItemsCount, Spacing), maxItemHeight);
        }

        double totalWidthWithoutSpacing = availableSize.Width - (Spacing * (visibleItemsCount - 1));
        maxItemWidth = totalWidthWithoutSpacing / visibleItemsCount;
        foreach (ref readonly UIElement child in visibleItems)
        {
            child.Measure(new(maxItemWidth, availableSize.Height));
            maxItemHeight = Math.Max(maxItemHeight, child.DesiredSize.Height);
        }

        return new(Math.Max(0, availableSize.Width), maxItemHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // Check if there's more (little) width available - if so, set max item width to the maximum possible as we have an almost perfect length.
        if (finalSize.Width > EqualPanelAlgorithm.GetTotalLength(maxItemWidth, visibleItemsCount, Spacing))
        {
            maxItemWidth = EqualPanelAlgorithm.GetItemLength(finalSize.Width, visibleItemsCount, Spacing);
        }

        int index = 0;
        double offset = 0;
        foreach (UIElement child in Children)
        {
            if (child.Visibility is Visibility.Collapsed)
            {
                continue;
            }

            child.Arrange(new(offset, 0, ++index == visibleItemsCount ? finalSize.Width - offset : maxItemWidth, maxItemHeight));
            offset += maxItemWidth + Spacing;
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

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        UnregisterPropertyChangedCallback(HorizontalAlignmentProperty, horizontalAlignmentChangedToken);
        Unloaded -= OnUnloaded;
    }
}
// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel;

[DependencyProperty<double>("MinItemWidth", NotNull = true)]
[DependencyProperty<double>("Spacing", NotNull = true)]
internal partial class HorizontalEqualPanel : Microsoft.UI.Xaml.Controls.Panel
{
    private Size effectiveSize;

    public HorizontalEqualPanel()
    {
        EffectiveViewportChanged += OnEffectiveViewportChanged;
        Unloaded += OnUnloaded;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        ReadOnlySpan<UIElement> visibleItems = [.. Children.Where(child => child.Visibility is Visibility.Visible)];
        int visibleItemsCount = visibleItems.Length;

        if (visibleItemsCount <= 0)
        {
            return default;
        }

        double minItemWidth = Math.Max(MinItemWidth, EqualPanelAlgorithm.GetItemLength(effectiveSize.Width, visibleItemsCount, Spacing));

        foreach (ref readonly UIElement child in visibleItems)
        {
            child.Measure(new(minItemWidth, effectiveSize.Height));
        }

        return new(Math.Max(effectiveSize.Width, EqualPanelAlgorithm.GetTotalLength(minItemWidth, visibleItemsCount, Spacing)), effectiveSize.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ReadOnlySpan<UIElement> visibleItems = [.. Children.Where(child => child.Visibility is Visibility.Visible)];
        double availableItemWidth = EqualPanelAlgorithm.GetItemLength(finalSize.Width, visibleItems.Length, Spacing);
        double actualItemWidth = Math.Max(MinItemWidth, availableItemWidth);

        double offset = 0;
        foreach (ref readonly UIElement visibleChild in visibleItems)
        {
            visibleChild.Arrange(new(offset, 0, actualItemWidth, effectiveSize.Height));
            offset += actualItemWidth + Spacing;
        }

        return new Size(Math.Max(0, offset - Spacing), effectiveSize.Height);
    }

    private void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        if (args.EffectiveViewport.IsEmpty)
        {
            return;
        }

        effectiveSize = args.EffectiveViewport.ToSize();
        effectiveSize.Width -= Margin.Left + Margin.Right;
        effectiveSize.Height -= Margin.Top + Margin.Bottom;

        InvalidateMeasure();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        EffectiveViewportChanged -= OnEffectiveViewportChanged;
        Unloaded -= OnUnloaded;
    }
}
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
            double availableWidth = double.IsInfinity(availableSize.Width)
                ? ActualWidth
                : availableSize.Width;

            double childAvailableWidth = (availableWidth + Spacing) / Children.Count;
            double childMaxAvailableWidth = Math.Max(MinItemWidth, childAvailableWidth);
            child.Measure(new(childMaxAvailableWidth, availableSize.Height));
        }

        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int itemCount = Children.Count;

        // 计算总间距
        double totalSpacing = Spacing * (itemCount - 1);

        // 添加间距后的总宽度
        double totalWidthWithSpacing = finalSize.Width - totalSpacing;

        // 计算每个子元素可用的宽度（考虑间距）
        double availableWidthPerItem = (totalWidthWithSpacing - totalSpacing) / itemCount;

        // 实际子元素宽度为最小宽度和可用宽度的较大值
        double actualItemWidth = Math.Max(MinItemWidth, availableWidthPerItem);

        double x = 0;

        // 设置子元素的位置和大小
        foreach (UIElement child in Children)
        {
            child.Arrange(new Rect(x, 0, actualItemWidth, finalSize.Height));
            x += actualItemWidth + Spacing;
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
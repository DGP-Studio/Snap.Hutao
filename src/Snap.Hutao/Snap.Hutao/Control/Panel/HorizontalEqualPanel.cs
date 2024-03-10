// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Data;
using System.Runtime.InteropServices;
using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

[DependencyProperty("MinItemWidth", typeof(double))]
[DependencyProperty("Spacing", typeof(double))]
internal partial class HorizontalEqualPanel : Microsoft.UI.Xaml.Controls.Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (UIElement child in Children)
        {
            double childAvailableWidth = (availableSize.Width + Spacing) / Children.Count;
            double childMinAvailableWidth = Math.Min(MinItemWidth, childAvailableWidth);
            child.Measure(new(childMinAvailableWidth, availableSize.Height));
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

            //child.Measure(new Size(actualItemWidth, finalSize.Height));
        }

        return finalSize;
    }
}
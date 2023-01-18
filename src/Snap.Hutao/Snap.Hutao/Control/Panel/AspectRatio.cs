// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 纵横比控件
/// </summary>
internal class AspectRatio : Microsoft.UI.Xaml.Controls.ContentControl
{
    private static readonly DependencyProperty TargetWidthProperty = Property<AspectRatio>.Depend(nameof(TargetWidth), 1D);
    private static readonly DependencyProperty TargetHeightProperty = Property<AspectRatio>.Depend(nameof(TargetHeight), 1D);

    /// <summary>
    /// 目标宽度
    /// </summary>
    public double TargetWidth
    {
        get => (double)GetValue(TargetWidthProperty);
        set => SetValue(TargetWidthProperty, value);
    }

    /// <summary>
    /// 目标高度
    /// </summary>
    public double TargetHeight
    {
        get => (double)GetValue(TargetHeightProperty);
        set => SetValue(TargetHeightProperty, value);
    }

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size availableSize)
    {
        double ratio = TargetWidth / TargetHeight;
        double ratioAvailable = availableSize.Width / availableSize.Height;

        // 更宽
        if (ratioAvailable > ratio)
        {
            double newWidth = ratio * availableSize.Height;
            return new Size(newWidth, availableSize.Height);
        }

        // 更高
        else if (ratioAvailable < ratio)
        {
            double newHeight = availableSize.Width / ratio;
            return new Size(availableSize.Width, newHeight);
        }

        return availableSize;
    }
}

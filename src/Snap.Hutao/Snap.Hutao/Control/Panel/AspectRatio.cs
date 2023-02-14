// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace Snap.Hutao.Control.Panel;

/// <summary>
/// 纵横比控件
/// </summary>
[HighQuality]
internal class AspectRatio : Microsoft.UI.Xaml.Controls.Control
{
    private const double Epsilon = 2.2204460492503131e-016;

    private static readonly DependencyProperty TargetWidthProperty = Property<AspectRatio>.DependBoxed<double>(nameof(TargetWidth), BoxedValues.DoubleOne);
    private static readonly DependencyProperty TargetHeightProperty = Property<AspectRatio>.DependBoxed<double>(nameof(TargetHeight), BoxedValues.DoubleOne);

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

        if (Math.Abs(ratioAvailable - ratio) < Epsilon)
        {
            return availableSize;
        }

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
